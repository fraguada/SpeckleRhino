<template>
  <div>
    <md-toolbar class="md-transparent">
      <span style="flex: 1"></span>
      <md-button class="md-icon-button md-raised md-primary" @click.native='showAddNewAccount=!showAddNewAccount'>
        <md-icon>{{ showAddNewAccount ? "close": "add" }}</md-icon>
        <md-tooltip md-direction="bottom">Add a new account</md-tooltip>
      </md-button>
      <span style="flex: 1"></span>
    </md-toolbar>

    <md-card class="paddedcard" v-show='showAddNewAccount'>
      <md-card-header>
        <div class="md-title">Register a new account.</div>
      </md-card-header>
      <md-card-content>
        <md-input-container>
          <label>Server URL</label>
          <md-input type="string"></md-input>
        </md-input-container>
        <md-input-container>
          <label>Email</label>
          <md-input type="string"></md-input>
        </md-input-container>
        <md-input-container>
          <label>Password</label>
          <md-input type="string"></md-input>
        </md-input-container>

      </md-card-content>
        <md-card-actions>
          <md-button class='md-primary'>Register</md-button>
        </md-card-actions>
    </md-card>
    <md-card class='paddedcard' v-for='account in allAccounts' :key="account.apiToken" style="">
      <md-card-header>
        <div class="md-subhead"><strong>{{account.serverName}}</strong></div>
        <div class="md-subhead">{{account.apiToken}}</div>
      </md-card-header>

      <md-card-content>
        {{ account.email }}<br>
        <a :href='account.rootUrl'> {{ account.rootUrl }} </a>
      </md-card-content>
      <md-card-actions>
        <md-button class='md-danger'>Ping</md-button>
        <md-button class='md-warn'>Remove</md-button>
      </md-card-actions>
    </md-card>

  </div>
</template>

<script>
export default {
  name: 'AccountList',
  computed: {
    allAccounts() {
      return this.$store.getters.allAccounts
    }
  },
  data () {
    return {
      showAddNewAccount: false
    }
  },
  methods: {
    openconsole () {
      console.log("hello world")
    }
  }
}
</script>

<style scoped>
.paddedcard {
  margin-top: 5px; margin-bottom:5px; width:90%; left: 5%; overflow:hidden;
}
</style>
