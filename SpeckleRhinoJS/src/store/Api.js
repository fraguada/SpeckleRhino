
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
        window.location.replace(args.streamId + '://' + 'togglelayer' + '/' + args.guid + '/' + args.visible)
    },
    layerColorUpdate(args) {
        window.location.replace(args.streamId + '://' + 'layerColorUpdate' + '/' + args.guid + '/' + args.color + '/' + args.opacity);
    },
    receiverReady(args)
    {
        window.location.replace(args.streamId + '://' + 'receiverReady' + '/' + args.name);
    },
    liveUpdate(args)
    {
        window.location.replace(args.streamId + '://' + 'liveUpdate');
    },
    metadataUpdate(args)
    {
        window.location.replace(args.streamId + '://' + 'metadataUpdate');
    }
}

export default SpkApi

window.SpkApi = SpkApi