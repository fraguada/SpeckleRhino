<template>
  <div id='color-picker' v-show='visible' ref='picker' v-drag:dragable>
    <div id="dragable">
      <md-button class="md-icon-button md-dense" style='margin:0;' @click.native='visible = false'>
        <md-icon style='font-size:20px;'>close</md-icon>
        <md-tooltip md-direction="bottom">Close</md-tooltip>
      </md-button>
    <!-- <compact-picker v-model='layerMaterial.color' class='actual-picker'></compact-picker> -->
      <div class='content'>
        <color-picker v-model='layerMaterial.color' class='actual-picker'></color-picker>
      </div>
    </div>
  </div>
</template>

<script>
import { Chrome, Compact, Slider }  from 'vue-color'
import * as THREE                   from 'three'
import debounce                     from 'debounce'

export default {
  name: '',
  components: {
    'color-picker': Chrome,
    'compact-picker': Slider
  },
  computed: {
    isGuestUser() {
      return false
    },
    layerMaterial() {
      if( this.layerGuid != '' )
        return this.$store.getters.layerMaterial( this.streamId, this.layerGuid )
      return this.temp
    },
    threeMeshMaterial() {
      return this.layerMaterial.threeMeshMaterial
    }
  },
  watch: {
    'layerMaterial.color': {
      handler( newValue ) {
        this.layerMaterial.threeMeshMaterial.color = new THREE.Color( newValue.hex )
        this.layerMaterial.threeLineMaterial.color = new THREE.Color( newValue.hex )
        this.layerMaterial.threePointMaterial.color = new THREE.Color( newValue.hex )
        this.layerMaterial.threeMeshMaterial.opacity = newValue.a
        this.layerMaterial.threeLineMaterial.opacity = newValue.a
        this.layerMaterial.threePointMaterial.opacity = newValue.a
      },
      deep: true
    },
    'visible': {
      handler( nval ) {
        if( !nval ) return this.commitUpdates( )
        console.log( this.$refs.picker ) 
        this.$refs.picker  
      }
    }
  },
  data() {
    return {
      temp: {
        color: { hex: '#B3B3B3', a: 1 },
        smooth: true,
        shiny: 0
      },
      layerGuid:'',
      streamId: '',
      visible: false,
      showExtra: false,
      pos: {}
    }
  },
  methods: {
    commitUpdates () {
      console.log( 'updating db with colors and stuffs.' )
      return console.warn('TODO')
      if( this.$store.getters.user.guest === true ) return console.warn('not authorised')
      this.$http.post( window.SpkAppConfig.serverDetails.restApi + '/streams/' + this.streamId + '/visuals',
        { 
          layerMaterials: this.$store.getters.receiverById( this.streamId ).layerMaterials 
        },
        { 
          headers: { Authorization : this.$store.getters.authToken }
      })
      .then( response => { } )
      .catch( err => {
        console.warn('Failed to update stream cosmetics.')
        console.warn( err )
      })
    }
  },
  mounted () {
    bus.$on( 'show-color-picker', args => {
      this.visible = ! this.visible
      this.layerGuid = args.layerGuid
      this.streamId = args.streamId
      this.pos = args.position
    } )
  }
}
</script>

<style>
  #color-picker{
    position: fixed;
    z-index: 100;
    left: 10px;
    top: 10px;
  }
  #dragable {
    position: relative;
    top: 0px;
    left: 0px;
    height: 30px;
    cursor: move;
    background-color: black;
    color: white;
    text-align: right;
    line-height: 30px;
  }
  .content {
    position: relative;
    top: -3px;
  }
</style>