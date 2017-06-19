<template>
  <div>
    <div class='paddedcard' style='position:relative;' v-show='allReceivers.length === 0'>
        <div class="md-title">There are no clients to show.</div>
        <p>You can add a new client by click on the big plus button above.</p>
    </div>
     <speckle-receiver v-for='receiver in allReceivers' :key="receiver.streamId" :spkreceiver='receiver'>
     </speckle-receiver>
  </div>
</template>

<script>
import SpeckleReceiver   from './SpeckleReceiver.vue'

export default {
  name: 'ReceiverList',
  components: {
    SpeckleReceiver
  },
  computed: {
    allReceivers() {
      return this.$store.getters.allReceivers
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