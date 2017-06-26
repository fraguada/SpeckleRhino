import Vue            from 'vue'
import VueMaterial    from 'vue-material'
import Vuex           from 'vuex'
import Axios          from 'axios'
import App            from './App.vue'
import Store          from './store/Store.js'
import vueDrag        from 'vue-dragging'

Vue.prototype.$http = Axios

Vue.use( VueMaterial )
Vue.use( vueDrag )
Vue.use( Vuex )

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
