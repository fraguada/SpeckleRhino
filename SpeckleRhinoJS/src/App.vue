<template>
  <div id="app">
    <md-toolbar class="md-transparent md-dense" style="margin-bottom:60px;">
      <span style="flex: 1"></span>
      <md-button class="md-icon-button md-right" @click.native="$refs.sidenav.toggle()">
        <md-icon>person</md-icon>
      </md-button>
      <md-button class="md-icon-button md-right" @click.native='openConsole'>
        <md-icon>bug_report</md-icon>
      </md-button>
    </md-toolbar>
    <md-sidenav class="md-right accounts-tab" ref="sidenav">
        <account-list></account-list>
    </md-sidenav>

    <md-speed-dial md-open="click" md-direction="right" class="md-fab-top-left " md-theme="default" v-if='currentView === "default"'>
      <md-button class="md-fab md-primary" md-fab-trigger>
        <md-icon md-icon-morph>close</md-icon>
        <md-icon>add</md-icon>
        <md-tooltip md-direction="bottom">Add a Speckle Client</md-tooltip>
      </md-button>

      <md-button class="md-fab md-primary md-mini md-clean" @click.native='createNewAccount=true'>
        <md-icon>cloud_download</md-icon>
        <md-tooltip md-direction="bottom">New Receiver</md-tooltip>
      </md-button>

      <md-button class="md-fab md-primary md-mini md-clean" :disabled="true">
        <md-icon @click='openConsole()'>cloud_upload</md-icon>
        <md-tooltip md-direction="bottom">New Sender</md-tooltip>
      </md-button>
      </md-speed-dial>
    
    <transition name="fade">
      <new-receiver v-show='createNewAccount===true' v-on:close='createNewAccount=false'></new-receiver>
    </transition>
    
    <receiver-list></receiver-list>
    <speckle-colour-panel></speckle-colour-panel>
  </div>
</template>

<script>
import AccountList          from './components/AccountList.vue'
import ReceiverList         from './components/ReceiverList.vue'
import NewReceiver          from './components/NewReceiver.vue'
import SpeckleColourPanel   from './components/SpeckleColourPanel.vue'

export default {
  name: 'app',
  components: {
    AccountList,
    ReceiverList,
    NewReceiver,
    SpeckleColourPanel
  },
  data () {
    return {
      currentView: 'default',
      createNewAccount: false
      // receivers: myReceiverStore
    }
  },
  computed: {
  },
  methods: {
    openConsole () {
      if( speckleRhinoPipeline )
          speckleRhinoPipeline.showDevTools()
    }
  },
  created () {
    this.$store.dispatch( 'getAllAccounts' )
    this.$store.dispatch( 'getAllReceivers' )
  }
}
</script>

<style>
  body{
    overflow-y: scroll;
  }
  #app {
    box-sizing: border-box;
    background-color: #E6E6E6;
  }
  .accounts-tab .md-sidenav-content{
    background-color: #E6E6E6;
  }

  .fade-enter-active, .fade-leave-active {
    transition: opacity .3s
  }
  .fade-enter, .fade-leave-to {
    opacity: 0
  }
</style>
