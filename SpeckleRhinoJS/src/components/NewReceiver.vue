<template>
  <md-card class='paddedcard' style="margin-bottom:20px;">
    <md-card-content>
    <h3>Create a new receiver</h3>
    <md-input-container>
      <label for='account'>Account</label>
      <md-select name='account' id='account' v-model='selectedAccount'>
        <md-option v-for='(account, index) in allAccounts' :key='account.apiToken' :value='index'><small> {{account.serverName}} ({{account.email}}) </small></md-option>
      </md-select>
    </md-input-container>
    <div v-show='selectedAccount!=-1'>
    <md-input-container>
      <label for="country">Your streams</label>
      <md-select name="stream" id="stream" v-model="selectedStream">
        <md-option v-for='stream in accountStreams' :value='stream.streamId' :key='stream.streamId'><small><strong>{{ stream.name }}</strong> | {{ stream.streamId}} | {{ stream.createdAt | formatDate }} </small></md-option>
        <md-option v-if='accountStreams.length == 0'>Please select an account first.</md-option>
      </md-select>
    </md-input-container>
     <div style='text-align:center'>OR</div>
     <br>
    <md-input-container>
      <label>other stream id</label>
      <md-input v-model='streamId'></md-input>
    </md-input-container>
    </div>

    <md-button v-show='selectedAccount!=-1' class='md-dense md-primary md-raised' @click.native='createReceiver'>Create Receiver</md-button>
    <md-button class='md-dense md-warnxxx md-raisedxx' @click.native='closeMyself'>Cancel</md-button>
  </div>
  </md-card-content>
  </md-card>

</template>
<script>

import axios        from 'axios'

export default {
  name: 'NewReceiver',
  computed: {
    allAccounts() {
      return this.$store.getters.allAccounts
    }
  },
  watch: {
    selectedAccount() {
      if( this.selectedAccount == -1 ) return
      axios.get( this.allAccounts[ this.selectedAccount ].restApi + '/streams' , { headers: { 'speckle-token' : this.allAccounts[ this.selectedAccount ].apiToken } } )
      .then( response => {
        this.accountStreams = response.data.data
      })
      .catch( err => {
        console.log( err )
      })
    },
    selectedStream( ) {
      if( this.selectedStream === '' ) return
      this.streamId = this.selectedStream
      this.createReceiver()
    }
  },
  data() {
    return {
      expandedCard: false,
      selectedAccount: -1,
      streamId: '',
      streams:[],
      selectedStream: '',
      accountStreams: []
    } 
  },
  methods: {
    createReceiver () { 
      console.log('called createReceiver')
      if( this.selectedAccount < 0 ) return alert( 'Please select an account to use.' )
      if( this.streamId === '' ) return alert( 'Please input a streamId.' )
      let account = this.allAccounts[ this.selectedAccount ]
      let receiver = { streamId: this.streamId, token: account.apiToken, serverUrl: account.rootUrl, restApi: account.restApi,  name: 'Loading...', layers:[], layerMaterials: [] }
      this.$store.commit( 'ADD_RECEIVER', { receiver } )
      this.closeMyself()
      // this.$emit( 'close' )
    },
    closeMyself() {
      this.selectedStream = ''
      this.selectedAccount = -1
      this.selectedStream = ''
      this.accountStreams = []
      this.$emit('close')
    }
  }
}
</script>
<style>
  .stream-list {
    max-height: 400px;
    overflow-y: scroll;
  }
</style>