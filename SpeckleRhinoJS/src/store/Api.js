
// dummy accounts
const _accounts = [{
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
}]


let SpkApi = {
    getAccounts(cb) {
        return typeof speckleRhinoPipeline != 'undefined' ? cb(JSON.parse(speckleRhinoPipeline.getAccounts())) : cb(_accounts)
    },
    addAccount(account, cb) {
        // todo
    },
    getReceivers(cb) {
        // return typeof speckleRhinoPipeline != 'undefined' ? cb( JSON.parse( speckleRhinoPipeline.getReceivers() ) ) : cb( _receivers )
    },
    toggleLayer(args) {
        var encodedArgs = window.btoa(args.streamId + '/' + args.guid + '/' + args.visible)
        window.location.replace('speckle' + '://' + 'togglelayer' + '/' + encodedArgs)
    },
    layerColorUpdate(args) {
        var encodedArgs = window.btoa(args.streamId + '/' + args.guid + '/' + args.color + '/' + args.opacity)
        window.location.replace('speckle' + '://' + 'layercolorupdate' + '/' + encodedArgs)
    },
    receiverReady(args)
    {
        var encodedArgs = window.btoa(args.streamId + '/' + args.apiUrl + '/' + args.token + '/' + args.name)
        window.location.replace('speckle' + '://' + 'receiverready' + '/' + encodedArgs);
    },
    liveUpdate(args)
    {
        var encodedArgs = window.btoa(args.streamId)
        window.location.replace('speckle' + '://' + 'liveupdate' + '/' + encodedArgs)
    },
    metadataUpdate(args)
    {
        var encodedArgs = window.btoa(args.streamId)
        window.location.replace('speckle' + '://' + 'metadataupdate' + '/' + encodedArgs)
    }
}

export default SpkApi

window.SpkApi = SpkApi