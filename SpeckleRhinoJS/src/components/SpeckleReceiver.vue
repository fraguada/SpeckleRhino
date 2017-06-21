<template>
<div>
  <md-card class="receiver paddedcard">
    <md-progress md-indeterminate v-show='showProgressBar'></md-progress>
    <md-progress :progress='objLoadProgress' v-show='objLoadProgress < 100 '></md-progress>
    <md-card-header style='line-heigth:30px'>
       <span class="md-body-2" style='line-heigth:30px'>
        <md-button class='md-icon-button md-dense' @click.native='expanded = ! expanded'>
          <md-icon>{{ expanded ? 'keyboard_arrow_up' : 'keyboard_arrow_down' }}</md-icon>
        </md-button>{{ spkreceiver.name }} 
      </span>
      <span class="md-caption" style='line-heigth:30px'><code style="user-select:all">{{ spkreceiver.streamId }}</code></span>
    </md-card-header>
    <div v-show='expanded'>
<!--       <md-card-actions>
        <md-button class='md-dense md-primary md-raised'>Refresh</md-button>
        <md-button class='md-dense md-warn'>Remove</md-button>
        <span style="flex: 1"></span>
      </md-card-actions> -->
      <md-card-content>
        <speckle-receiver-layer v-for='layer in spkreceiver.layers' :key='layer.guid' :spklayer='layer' :streamid='spkreceiver.streamId'></speckle-receiver-layer>
      </md-card-content>
    </div>
    <!-- </md-card-expand> -->
  </md-card>
</div>
</template>

<script>
import axios                      from 'axios'
import ReceiverClient             from '../receiver/SpeckleReceiver'

import SpeckleReceiverLayer       from './SpeckleReceiverLayer.vue'


export default {
  name: 'SpeckleReceiver',
  components: {
    SpeckleReceiverLayer
  },
  props: ['spkreceiver'],
  data () {
    return {
      showProgressBar: true,
      objLoadProgress: 100,
      showError: true,
      error: 'No errors.',
      name:'loading...',
      expanded: true
    }
  },
  methods: {
    receiverError( err ) {
      this.error = err
    },
    receiverReady( name, layers, objects, history, layerMaterials ) {
      this.showProgressBar = false
      this.objLoadProgress = 0
      let payload = { streamId: this.spkreceiver.streamId, name: name, layers: layers, objects: objects, layerMaterials: layerMaterials }
      this.$store.commit( 'INIT_RECEIVER_DATA',  { payload } )
    },
    liveUpdate( name, layers, objects, objectProperties ) {
      this.showProgressBar = false
      this.objLoadProgress = 0
      this.name = name

      let payload = { streamId: this.spkreceiver.streamId, name: name, layers: layers, objects: objects }
      this.$store.commit( 'SET_RECEIVER_DATA',  { payload } )

      this.mySpkReceiver.getObjects( ( objs ) => {
        if( typeof cefCustomObject != 'undefined' ) 
          cefCustomObject.liveUpdate( this.spkreceiver.streamId, JSON.stringify( objs ), JSON.stringify(objectProperties ) )
      })
    }, 
    metadataUpdate( name, layers ) {
      let payload = { streamId: this.spkreceiver.streamId, name: name, layers: layers }
      this.$store.commit( 'SET_RECEIVER_METADATA',  { payload } )
    },
    historyUpdate( ) {
      // todo
    },
    objLoadProgressEv( loaded ) {
      this.objLoadProgress = (loaded+1) / this.objects.length * 100
    }
  },
  created() {
    console.log("I WAS CREATED MUHAHAHAHAH")
    
    this.mySpkReceiver = new ReceiverClient({
      serverUrl: this.spkreceiver.serverUrl,
      streamId: this.spkreceiver.streamId,
      token: this.spkreceiver.token
    })

    this.mySpkReceiver.on( 'error', this.receiverError )
    this.mySpkReceiver.on( 'ready', this.receiverReady )
    this.mySpkReceiver.on( 'live-update', this.liveUpdate )
    this.mySpkReceiver.on( 'metadata-update', this.metadataUpdate )
    this.mySpkReceiver.on( 'history-update', this.historyUpdate )
    this.mySpkReceiver.on( 'object-load-progress', this.objLoadProgressEv ) 
  }
}
</script>

<style>
.receiver {
  margin-bottom: 10px;
}
</style>
