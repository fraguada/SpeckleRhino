import Vue from 'vue'
import VueMaterial from 'vue-material'
import Vuex from 'vuex'
import App from './App.vue'
import Store from './store/Store.js'

Vue.use( VueMaterial )
Vue.use( Vuex )

// const rh = cefCustomObject
window.bus = new Vue( )

Vue.material.registerTheme( 'default' , {
  primary: 'black',
  accent: 'blue',
  warn: 'red',
  background: 'white'
})

import moment from 'moment'

Vue.filter('formatDate', function(value) {
  if (value) {
    return moment(String(value)).format('MM/DD/YYYY')
  }
})

new Vue({
  el: '#app',
  store: Store,
  render: h => h(App)
})
