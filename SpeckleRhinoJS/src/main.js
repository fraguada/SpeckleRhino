import Vue from 'vue'
import VueMaterial from 'vue-material'
import Vuex from 'vuex'
import App from './App.vue'
import Store from './store/Store.js'

Vue.use( VueMaterial )
Vue.use( Vuex )

// const rh = cefCustomObject

Vue.material.registerTheme( 'default' , {
  primary: 'black',
  accent: 'blue',
  warn: 'red',
  background: 'white'
})

new Vue({
  el: '#app',
  store: Store,
  render: h => h(App)
})
