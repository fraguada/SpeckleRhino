<template>
  <div class='spk-layer'>
    <div class='layer-details'>
    <span class="layer-name">
      {{ spklayer.name }} ({{spklayer.objectCount}} objs)
    </span>
    <span class="layer-buttons"> 
      <!-- <md-icon @click.native='showColorPicker'><md-tooltip>Bake Layer</md-tooltip>file_download</md-icon> -->
      <md-icon @click.native='showColorPicker' :style='colorStyle'><md-tooltip>Change color</md-tooltip>color_lens</md-icon>
      <md-icon @click.native='toggleLayer'><md-tooltip>Toggle Visibility</md-tooltip>{{ visible ? "visibility" : "visibility_off" }}</md-icon>
    </span>
    </div>
  </div>
</template>

<script>
export default {
  name: 'SpkReceiverLayer',
  props: { 
    spklayer: { type: Object },
    streamid: { type: String }
  },
  components: {
  },
  computed: {
    layerMaterial() {
      return this.$store.getters.layerMaterial( this.streamid, this.spklayer.guid )
    },
    colorStyle() {
      if( this.layerMaterial )
        return 'color:' + this.layerMaterial.color.hex
      return 'color:gray'
    },
    showPicker() {
      return this.showColorPicker
    }
  },
  data() {
    return {
      visible: true
    }
  },
  methods: {
    showColorPicker() {
      // bus.$emit( 'show-color-picker', { layerGuid: this.spklayer.guid, streamId: this.streamid } )
    },
    toggleLayer() {
      this.visible = ! this.visible
       if( typeof cefCustomObject != 'undefined' ) 
          cefCustomObject.layerToggle( JSON.stringify( { visible: this.visible, layerGuid: this.spklayer.guid, streamId: this.streamId } ) )
    }
  }, 
  mounted() {
  }
}
</script>

<style scoped>

.spk-layer{
  border-bottom: 1px solid #E6E6E6;
  position: relative;
  user-select: none;
}
.layer-details {
  line-height: 50px;
  font-size: 12px;
  height: 50px;
}
.layer-name {
  float: left;
  display: inline-block;
  width: 50%;
  overflow: hidden;
}
.layer-buttons {
  /*padding-top: 5px;*/
  text-align: right;
  float: left;
  display: inline-block;
  width: 50%;
  box-sizing: border-box;
  color: #666666;
  /*cursor: pointer;*/
}

.layer-buttons .md-icon {
  cursor: pointer;
}

</style>