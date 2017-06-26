<template>
  <div id='color-picker' v-show='visible'>
    <div id="dragable">
      <md-button class="md-icon-button md-dense" style='margin:0;' @click.native='visible = false'>
        <md-icon style='font-size:20px;'>close</md-icon>
        <md-tooltip md-direction="bottom">Close</md-tooltip>
      </md-button>
    </div>
    <div class="content">
      <color-picker v-model='layerMaterial.color'></color-picker>
    </div>
  </div>
</template>

<script>
import { Chrome, Compact }          from 'vue-color'
import * as THREE                   from 'three'
import debounce                     from 'debounce'

export default {
  name: '',
  components: {
    'color-picker': Chrome,
    'compact-picker': Compact
  },
  computed: {
    isGuestUser() {
      return this.$store.getters.user.guest
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
    'layerMaterial.shininess': {
      handler( newValue ) {
        if( newValue < 0 ) newValue = 0
        if( newValue > 50 ) newValue = 50
        this.layerMaterial.threeMeshMaterial.shininess = newValue
        this.layerMaterial.threeMeshVertexColorsMaterial.shininess = newValue
      }
    },
    'layerMaterial.showEdges': {
      handler( newValue ) {
        this.layerMaterial.threeEdgesMaterial.visible = newValue
      }
    },
    'layerMaterial.wireframe': {
      handler( newValue ) {
        this.layerMaterial.threeMeshMaterial.wireframe = newValue
        this.layerMaterial.threeMeshVertexColorsMaterial.wireframe = newValue
      }
    },
    // 'layerMaterial.vertexColors': {
    //   handler( newValue ) {
    //     this.layerMaterial.threeMeshMaterial.vertexColors = newValue
    //     bus.$emit('renderer-layer-update-colors', { layerGuid: this.layerGuid, streamId: this.streamId } )
    //     // this.layerMaterial.threeMeshMaterial.
    //   }
    // },
    'visible': {
      handler( nval ) {
        if( !nval ) this.commitUpdates( )
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
      showExtra: false
    }
  },
  methods: {
    commitUpdates () {
      console.log( 'updating db with colors and stuffs.' )
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
    } )
  }
}
</script>

<style>
#dragable {
    position: relative;
    top: 0px;
    left: 0px;
    height: 30px;
    cursor: move;
    background-color: white;
    text-align: right;
    line-height: 30px;
}
#dragable span {
  /*width: 220px;*/
  line-height: 30px;
  cursor: pointer;
}
#color-picker{
  position: fixed;
  z-index: 100;
  left: 420px;
  top: 10px;
}
.other-options{
  border-top: 1px solid #E6E6E6;
  box-sizing: border-box;
  padding-left: 15px;
  padding-right: 15px;
}

.content {
  background-color: white;
}

</style>