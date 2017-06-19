
// dummy accounts
const _accounts = [ {
  apiToken: "faa6ba741e854d6e8fe6ae66218ac736",
  email: "d.stefanescu@ucl.ac.uk",
  restApi: "https://server.speckle.works/api/v1",
  rootUrl: "https://server.speckle.works/",
  serverName: "Speckle Test Deployment - Speckle.Works!"
}, {
  apiToken: "lol",
  email: "d.stefanescu@ucl.ac.uk",
  restApi: "https://server.speckle.works/api/v1",
  rootUrl: "https://server.speckle.works/",
  serverName: "AnotherOne"
} ]

const _receivers = [] 
// const _receivers = [ {
//   serverUrl: 'https://server.speckle.works/',
//   streamId: 'ByLzkIOxZ',
//   token: 'faa6ba741e854d6e8fe6ae66218ac736'
// } ]


export default {
  getAccounts( cb ) {
    return typeof cefCustomObject != 'undefined' ? cb( JSON.parse( cefCustomObject.getAccounts() ) ) : cb( _accounts )
  },
  addAccount( account, cb) {
    // todo
  },
  getReceivers( cb ) {
    cb( _receivers)
    // return typeof cefCustomObject != 'undefined' ? cb( JSON.parse( cefCustomObject.getReceivers() ) ) : cb( _receivers )
  }
}