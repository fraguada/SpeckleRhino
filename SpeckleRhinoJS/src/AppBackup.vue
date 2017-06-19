<template>
  <div id="app">
    <!-- <md-toolbar class="md-large"></md-toolbar> -->

  <md-toolbar class="md-dense">
    <div class="md-toolbar-container">
      <md-button class="md-fabxx md-mini" @click.native='currentView="default"'>
        <md-icon>home</md-icon>
      </md-button>
      <md-button class="md-mini md-clean" @click.native='currentView="accounts"'>
        <md-icon>person</md-icon>
        <md-tooltip md-direction="bottom">Manage Accounts</md-tooltip>
      </md-button>
      <md-button class="md-mini md-clean" @click.native='openConsole'>
        <md-icon>bug_report</md-icon>
        <md-tooltip md-direction="bottom">Open Console</md-tooltip>
      </md-button>
    </div>
  </md-toolbar>

    <md-speed-dial md-open="click" md-direction="top" class="md-fab-bottom-left" md-theme="default" v-if='currentView === "default"'>
      <md-button class="md-fab" md-fab-trigger>
        <md-icon md-icon-morph>close</md-icon>
        <md-icon>add</md-icon>
        <md-tooltip md-direction="bottom">Add a Speckle Client</md-tooltip>
      </md-button>

      <md-button class="md-fab md-primary md-mini md-clean">
        <md-icon>cloud_download</md-icon>
        <md-tooltip md-direction="bottom">New Receiver</md-tooltip>
      </md-button>

      <md-button class="md-fab md-primary md-mini md-clean" :disabled="true">
        <md-icon @click='openConsole()'>cloud_upload</md-icon>
        <md-tooltip md-direction="bottom">New Sender</md-tooltip>
      </md-button>

      <md-button class="md-fab md-warn md-mini md-clean" @click.native="openConsole">
        <md-icon>bug_report</md-icon>
        <md-tooltip md-direction="bottom">Open Console</md-tooltip>
      </md-button>
    </md-speed-dial>
    
    <account-list v-if='currentView === "accounts"'></account-list>
  </div>
</template>

<script>
import AccountList from './components/AccountList.vue'
import Receiver from './components/Receiver.vue'
// import { mapGetter } from ''

export default {
  name: 'app',
  components: {
    AccountList,
    Receiver
  },
  data () {
    return {
      currentView: 'default'
      // receivers: myReceiverStore
    }
  },
  computed: {
  },
  methods: {
    openConsole () {
      if( cefCustomObject )
        cefCustomObject.showDevTools()
    }
  },
  created () {
    this.$store.dispatch( 'getAllAccounts' )
  }
}
</script>

<style>
#app {
  box-sizing: border-box;
  /*padding-top: 100px;*/
/*  padding-left: 15px;
  padding-right: 15px;*/
  background-color: #E6E6E6;
}
</style>
