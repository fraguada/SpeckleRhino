import Vue          from 'vue'
import Vuex         from 'vuex'
import Api          from './Api.js'

import LMat         from './LayerMaterial'
import * as THREE   from 'three'
Vue.use(Vuex)

export default new Vuex.Store( {
  state: { 
    accounts: [], //JSON.parse( cefCustomObject.getAccounts() )
    receivers: [],
    comments: [],
  },
  getters: {
    allAccounts: state =>  state.accounts,
    getAccountByIndex: state => ( index ) => {
      return state.accounts[ index ]
    },
    allReceivers: state => state.receivers,
    receiverById: state => ( streamId ) => {
      return state.receivers.find( rec => rec.streamId === streamId )
    },
    receiverLayers: state => ( streamId ) => {
      return state.receivers.find( rec => rec.streamId === streamId ).layers
    },
    layerMaterial: ( state, getters ) => ( streamId, layerGuid ) => {
      return getters.receiverById( streamId ).layerMaterials.find( item => item.guid === layerGuid )
    },
    allLayerMaterials: ( state ) => {
      let arr = []
      state.receivers.forEach( rec => arr.push( rec.layerMaterials ) )
      return arr.concat.apply( [], arr )
    },
    receiverComments: state => ( streamId ) => {
      return state.comments.filter( comment => comment.streamId === streamId ).reverse()
    },
  },
  actions: {
    getAllAccounts ( { commit } ) {
      Api.getAccounts( accounts => {
        commit( 'RECEIVE_ACCOUNTS', { accounts } )
      })
    },
    getAllReceivers( { commit } ) {
      console.log('woa')
      Api.getReceivers( receivers => {
        commit( 'RECEIVE_RECEIVERS', { receivers })
      })
    }
  },
  mutations: {
    RECEIVE_ACCOUNTS ( state, { accounts } ) {
      state.accounts = accounts
    },

    RECEIVE_RECEIVERS( state, { receivers } ) { 
      state.receivers = receivers
    },

    ADD_RECEIVER( state, { receiver } ) {
      state.receivers.push( receiver )
    },
    
    ADD_COMMENT( state, { payload } ) {
      state.comments.push( payload )
    },

    ADD_COMMENTS( state, { payload } ) {
      state.comments.push( ...payload.comments )
    },

    INIT_RECEIVER_DATA( state, { payload } ) {
      let target = state.receivers.find( rec => rec.streamId === payload.streamId )
      target.name = payload.name
      target.layers = payload.layers
      
      payload.objects.forEach( ( obj, index )=> { 
        obj.streamId = payload.streamId 
        obj.layerGuid = payload.layers.find( layer => {
          return index >= layer.startIndex && index < layer.startIndex + layer.objectCount
        }).guid
      } )
      target.objects = payload.objects
      
      if( !payload.layerMaterials ) { // create layerMaterials
        target.layerMaterials = []
        target.layers.forEach( layer => {
          target.layerMaterials.push( new LMat( { guid: layer.guid, streamId: target.streamId } ) )
        })
      } else {  // check completion
        target.layerMaterials = payload.layerMaterials
        target.layers.forEach( layer => { 
          let myLMat = target.layerMaterials.find( obj => { return layer.guid === obj.guid } )
          if( ! myLMat )
            target.layerMaterials.push( new LMat( { guid: layer.guid, streamId: target.streamId } ) )
          else {
            // recreate the materials for mr. threejs because fuck you too
            myLMat.threeMeshMaterial = new THREE.MeshPhongMaterial( { ...myLMat.threeMeshMaterial } ) 
            myLMat.threeMeshMaterial.vertexColors = false
            myLMat.threeLineMaterial = new THREE.LineBasicMaterial( { ...myLMat.threeLineMaterial } )
            myLMat.threeEdgesMaterial = new THREE.LineBasicMaterial( { ...myLMat.threeEdgesMaterial } )
            myLMat.threeEdgesMaterial.visible = myLMat.showEdges
            myLMat.threePointMaterial = new THREE.PointsMaterial( { ...myLMat.threePointMaterial } )

            //backwards compatibility hack for colored meshes
            if( myLMat.threeMeshVertexColorsMaterial )
              myLMat.threeMeshVertexColorsMaterial = new THREE.MeshPhongMaterial( { ...myLMat.threeMeshVertexColorsMaterial } ) 
            else 
              myLMat.threeMeshVertexColorsMaterial = new LMat({ guid: layer.guid, streamId: target.streamId }).threeMeshVertexColorsMaterial
          }
        } ) 
      }
    },

    SET_RECEIVER_METADATA( state, { payload } ) {
      let target = state.receivers.find( rec => rec.streamId === payload.streamId )
      target.name = payload.name
      target.layers = payload.layers
      
      //check for layermaterials completion
      target.layers.forEach( layer => { 
        if( ! target.layerMaterials.find( obj => { return layer.guid === obj.guid } ) )
          target.layerMaterials.push( new LMat( { guid: layer.guid, streamId: target.streamId } ) )
      } ) 
    },

    SET_RECEIVER_DATA( state, { payload } ) {

      let target = state.receivers.find( rec => rec.streamId === payload.streamId )
      target.name = payload.name
      target.layers = payload.layers
      
      payload.objects.forEach( ( obj, index )=> { 
        obj.streamId = payload.streamId 
        obj.layerGuid = payload.layers.find( layer => {
          return index >= layer.startIndex && index < layer.startIndex + layer.objectCount
        }).guid
      } )
      target.objects = payload.objects
      
      //check for layermaterials completion
      target.layers.forEach( layer => { 
        if( ! target.layerMaterials.find( obj => { return layer.guid === obj.guid } ) )
          console.warn('missing layer.')
          target.layerMaterials.push( new LMat( { guid: layer.guid, streamId: target.streamId } ) )
      } ) 
    }
  },
})