import Vue from 'vue'
import Vuex from 'vuex'
import Api from './Api.js'

Vue.use(Vuex)

export default new Vuex.Store( {
  state: { 
    accounts: [], //JSON.parse( cefCustomObject.getAccounts() )
    receivers: []
  },
  getters: {
    allAccounts: state =>  state.accounts,
    getAccountByIndex: state => ( index ) => {
      return state.accounts[ index ]
    },
    allReceivers: state => state.receivers
  },
  actions: {
    getAllAccounts ( { commit } ) {
      Api.getAccounts( accounts => {
        commit( 'RECEIVE_ACCOUNTS', { accounts } )
      })
    },
    getAllReceivers( { commit } ) {
      console.log('woa')
      Api.getReceivers( receivers => {
        commit( 'RECEIVE_RECEIVERS', { receivers })
      })
    }
  },
  mutations: {
    RECEIVE_ACCOUNTS ( state, { accounts } ) {
      state.accounts = accounts
    },
    RECEIVE_RECEIVERS( state, { receivers } ) { 
      state.receivers = receivers
    },
    ADD_RECEIVER( state, { receiver } ) {
      console.log( receiver )
      state.receivers.push( receiver )
    }
  },
})