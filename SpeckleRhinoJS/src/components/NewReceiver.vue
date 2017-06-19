<template>
  <md-card class='paddedcard' style="margin-bottom:20px;">
  <md-card-content>
    <md-input-container>
      <label for='account'>Account</label>
      <md-select name='account' id='account' v-model='selectedAccount'>
        <md-option v-for='(account, index) in allAccounts' :key='account.apiToken' :value='index'> {{account.serverName}} <br> <small>{{account.email}} </small></md-option>
      </md-select>
    </md-input-container>
    <md-input-container>
      <label>Stream Id</label>
      <md-input v-model='streamId'></md-input>
    </md-input-container>
    <md-button class='md-dense md-primary md-raised' @click.native='createReceiver'>Create Receiver</md-button>
    <md-button class='md-dense md-warnxxx md-raisedxx' @click.native='closeMyself'>Cancel</md-button>
  </md-card-content>
  </md-card>

</template>
<script>
export default {
  name: 'NewReceiver',
  computed: {
    allAccounts() {
      return this.$store.getters.allAccounts
    }
  },
  data() {
    return {
      selectedAccount: -1,
      streamId: '',
    } 
  },
  methods: {
    createReceiver () { 
      if( this.selectedAccount < 0 ) return alert( 'Please select an account to use.' )
      if( this.streamId === '' ) return alert( 'Please input a streamId.' )
      let account = this.allAccounts[ this.selectedAccount ]
      let receiver = { streamId: this.streamId, token: account.apiToken, serverUrl: account.rootUrl }
      console.log(receiver)
      this.$store.commit( 'ADD_RECEIVER', { receiver } )
      this.$emit( 'close' )
    },
    closeMyself() {
      this.$emit('close')
    }
  }
}
</script>