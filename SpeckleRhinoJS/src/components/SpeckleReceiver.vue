<template>
<div>
  <md-card class="receiver paddedcard">  
    <md-card-header style='line-heigth:30px' class='line-height-adjustment'>
      <span class="md-body-2">
        <md-button class='md-icon-button md-dense xxxmd-accent xxxmd-raised' @click.native='expanded = ! expanded'>
          <md-icon>{{ expanded ? 'keyboard_arrow_up' : 'keyboard_arrow_down' }}</md-icon>
        </md-button>{{ spkreceiver.name }} 
      </span>
      <span class="md-caption"><code style="user-select:all">{{ spkreceiver.streamId }}</code></span>
      <span class='option-buttons' style='float:right'>
      <md-icon>visibility</md-icon>
      <md-icon>close</md-icon>
      </span>
      <br>
      <md-progress md-indeterminate v-show='showProgressBar' style='margin-bottom:20px;margin-top:20px;'></md-progress>
      <!-- <div class="md-caption"><br>ID: <code>{{ spkreceiver.streamId }}</code></div> -->
    </md-card-header>
    <md-card-content v-show='expanded'>
      <md-tabs md-fixed class='md-transparent'>
        <md-tab id="layers" md-label="layers" class='receiver-tabs'>
            <speckle-receiver-layer v-for='layer in layers' :key='layer.guid' :spklayer='layer' :streamid='spkreceiver.streamId'></speckle-receiver-layer>
        </md-tab>
        <md-tab id='comments' md-label='views' class='receiver-tabs'>
          <speckle-receiver-comments :streamid='spkreceiver.streamId' xxxv-on:comment-submit='commentSubmit' ></speckle-receiver-comments>
        </md-tab>
        <md-tab id='versions' md-label='versions' class='receiver-tabs'>
        <br>
        <div class="md-subhead">Todo.</div>
<!--           <speckle-receiver-comments :streamid='spkreceiver.streamId' v-on:comment-submit='commentSubmit' ></speckle-receiver-comments> -->
        </md-tab>
      </md-tabs>
      
    </md-card-content>
  </md-card>
</div>
</template>

<script>
import ReceiverClient             from '../receiver/SpeckleReceiver' // temporary solution to fix uglify error on build.
import SpeckleReceiverComments    from './SpeckleReceiverComments.vue'
import SpeckleReceiverLayer       from './SpeckleReceiverLayer.vue' 
import SpkApi                     from '../store/Api'

export default {
  name: 'SpeckleReceiver',
  components: {
    SpeckleReceiverLayer,
    SpeckleReceiverComments
  },
  props: [ 'spkreceiver' ],
  computed: {
    username() {
      return this.$store.getters.user.name
    },
    layers() {
      return this.spkreceiver.layers
    }
  },
  data () {
    return {
      showProgressBar: true,
      objLoadProgress: 100,
      objListLength: 1,
      comments: 'Hello World. How Are you? Testing testing 123.',
      isStale: false,
      expanded: true,
      visible: true
    }
  },
  methods: {
    receiverError( err ) {
      this.errror = err
    },
    getComments( ) {
      this.$http.get( this.spkreceiver.restApi + '/comments/' + this.spkreceiver.streamId )
      .then( response => {
        if( !response.data.success ) throw new Error( 'Failed to retrieve comments for stream ' + this.spkreceiver.streamId )
        // if( response.data.comments.length <= 0 ) return console.warn( 'Stream had no commnets.' )
        let payload = { comments: response.data.comments }
        this.$store.commit( 'ADD_COMMENTS', { payload } )
      })
      .catch( err => {
        console.warn( err )
      })
    },
    receiverReady( name, layers, objects, history, layerMaterials ) {
      console.info('Receiver ready', this.spkreceiver.streamId )
      this.getComments() 
      this.showProgressBar = false
      this.objLoadProgress = 0
      this.objListLength = objects.length

      let payload = { streamId: this.spkreceiver.streamId, name: name, layers: layers, objects: objects, layerMaterials: layerMaterials }
      this.$store.commit( 'INIT_RECEIVER_DATA',  { payload } )
  
      this.isStale = true
      var url = new URL(this.spkreceiver.serverUrl)
      SpkApi.receiverReady({ apiUrl: url.hostname, token: this.spkreceiver.token, streamId: this.spkreceiver.streamId, name: name })
      //this.mySpkReceiver.getObjects( ( objs ) => {

      //   // if (typeof speckleRhinoPipeline != 'undefined')
      //    //     speckleRhinoPipeline.liveUpdate(this.spkreceiver.streamId, name, JSON.stringify(objs), JSON.stringify(this.spkreceiver.objectProperties), JSON.stringify(layers), JSON.stringify(this.spkreceiver.layerMaterials))
          
          
      //})
    },
    liveUpdate( name, layers, objects, objectProperties ) {
      console.info('Live update', this.spkreceiver.streamId )
      this.showProgressBar = false
      this.objLoadProgress = 0
      this.objListLength = objects.length

      let payload = { streamId: this.spkreceiver.streamId, name: name, layers: layers, objects: objects }
      this.$store.commit( 'SET_RECEIVER_DATA',  { payload } )
      this.isStale = true
      SpkApi.liveUpdate({ streamId: this.spkreceiver.streamId });
      //this.mySpkReceiver.getObjects( ( objs ) => {

      ////    if (typeof speckleRhinoPipeline != 'undefined')             
      ////        speckleRhinoPipeline.liveUpdate(this.spkreceiver.streamId, name, JSON.stringify(objs), JSON.stringify(objectProperties), JSON.stringify(layers), JSON.stringify(this.spkreceiver.layerMaterials))

         

      //})
    },
    metadataUpdate( name, layers ) {
      console.info('Metadata update', this.spkreceiver.streamId )
      let payload = { streamId: this.spkreceiver.streamId, name: name, layers: layers }
      this.$store.commit( 'SET_RECEIVER_METADATA',  { payload } )
      //if( typeof speckleRhinoPipeline != 'undefined' ) 
       //   speckleRhinoPipeline.metadataUpdate(name, JSON.stringify(layers))

      SpkApi.metadataUpdate({ streamId: this.spkreceiver.streamId, name: name, layers: layers });
    },
    objLoadProgressEv( loaded ) {
      this.objLoadProgress = ( loaded + 1 ) / this.objListLength * 100
    },
    commentSubmit( comment ) {
      // let payload = comment
      // payload.streamId = this.spkreceiver.streamId

      // this.$http.post( window.SpkAppConfig.serverDetails.restApi + '/comments', { comment: comment }, { headers: { Authorization: this.$store.getters.authToken } } )
      // .then( response => {
      //   if( ! response.data.success ) throw new Error( 'Failed to post comment for a reason.' )
      //   this.$store.commit( 'ADD_COMMENT', { payload } )
      //   this.mySpkReceiver.broadcast(  { event: 'comment-added',  comment: comment } )
      // })
      // .catch( err => { 
      //   console.log( err ) 
      // })
    },
    broadcastReceived( message ) {
      // console.log( message )
      // let parsedMessage = JSON.parse( message.args )
      // console.log( parsedMessage )
      // if( parsedMessage.event != 'comment-added' ) return
      // let payload = parsedMessage.comment
      // this.$store.commit( 'ADD_COMMENT', { payload } )
    },
    toggleVisibility( ) {
      // TODO
    }
  },
  mounted() {
    console.log( 'Receiver mounted: ' + this.spkreceiver.streamId )
    this.name = 'loading ' + this.spkreceiver.streamId
    this.mySpkReceiver = new ReceiverClient({
      serverUrl: this.spkreceiver.serverUrl,
      streamId: this.spkreceiver.streamId,
      token: this.spkreceiver.token
    })

    this.mySpkReceiver.on( 'error', this.receiverError )
    this.mySpkReceiver.on( 'ready', this.receiverReady )
    this.mySpkReceiver.on( 'live-update', this.liveUpdate )
    this.mySpkReceiver.on( 'metadata-update', this.metadataUpdate )
    this.mySpkReceiver.on( 'object-load-progress', this.objLoadProgressEv )
    this.mySpkReceiver.on( 'volatile-broadcast', this.broadcastReceived )
  }
}
</script>

<style>
.option-buttons {
  color: #808080 !important;
}
.line-height-adjustment{
  line-height: 34px;
}
.receiver {
  margin-bottom: 10px;
}
.receiver-tabs {
  padding: 0 !important;
}
</style>
