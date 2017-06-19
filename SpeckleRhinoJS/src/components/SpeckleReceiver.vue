<template>
<div>
  <md-card class="receiver paddedcard">
    <md-progress md-indeterminate v-show='showProgressBar'></md-progress>
    <md-progress :progress='objLoadProgress' v-show='objLoadProgress < 100 '></md-progress>
    
    <md-card-header>
      <div class="md-title">{{name}}</div>
    </md-card-header>
    <!-- <md-card-expand> -->
      <md-card-actions>
        <md-button class='md-dense md-primaryx md-raised'>Refresh</md-button>
        <md-button class='md-dense md-warn'>Remove</md-button>
        <span style="flex: 1"></span>
      </md-card-actions>
      <md-card-content>
        <div v-for='layer in layers'>
        {{ layer.name }} | {{ layer.objectCount }} objects 
        </div>
      </md-card-content>
    <!-- </md-card-expand> -->
  </md-card>
</div>
</template>

<script>
import ReceiverClient             from '../receiver/SpeckleReceiver' // temporary solution to fix uglify error on build.


export default {
  name: 'SpeckleReceiver',
  props: ['spkreceiver'],
  data () {
    return {
      showProgressBar: true,
      objLoadProgress: 100,
      showError: true,
      error: 'No errors.',
      name:'loading...',
      layers:[],
      objects:[],
      history:[]
    }
  },
  methods: {
    receiverError( err ) {
      this.errror = err
    },
    liveUpdate( name, layers, objects, history ) {
      this.showProgressBar = false
      this.objLoadProgress = 0
      this.name = name
      this.layers = layers
      this.objects = objects
      this.history = history

      this.mySpkReceiver.getObjects( ( objs ) => {
        console.log('Got all objects from server.')
        if( typeof cefCustomObject != 'undefined' ) // this is a quick hack for in browser quick `n dirty checks
          cefCustomObject.liveUpdate(  )
          cefCustomObject.addObjects( JSON.stringify(objs) )
      })
    },
    metadataUpdate( name, layers ) {
      this.name = name
      this.layers = layers
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
    this.mySpkReceiver.on( 'ready', this.liveUpdate )
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
