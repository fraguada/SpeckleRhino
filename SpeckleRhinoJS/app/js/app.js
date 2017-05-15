$(function () {

    var panelList = $('.sortable');

    panelList.sortable({
        handle: '.nodeHeader',
        containment: 'parent',
        update: function () {
            $('.sortableContainer', panelList).each(function (index, elem) {
                var $listItem = $(elem),
                    newIndex = $listItem.index();
            });
        }
    });
});

//$(window).load(function () {
$(window).on('load', function () {

    var cnt = 0;
    var receivers = [];

    $('#AddSender').click(function () {
        console.log('Adding Sender');
        var senderBlock = '<div class="sortableContainer"><a id="sortableNode" href="#demo' + cnt + '" class="list-group-item nodeHeader" data-toggle="collapse"><h5>Sender' + cnt + ' Id: xxxxx</h5></a><div class="collapse nodeItem" id= "demo' + cnt + '" ><a href="#" class="list-group-item">Subitem 1</a><a href="#" class="list-group-item">Subitem 2</a><a href="#" class="list-group-item">Subitem 3</a></div ></div >';
        $(senderBlock).appendTo("#SenderContainer");
        cnt++;
    });

    $('#AddReceiver').click(function () {
        console.log('Adding Receiver');
        var receiverBlock = '<div class="sortableContainer"><a id="sortableNode" href="#demo' + cnt + '" class="list-group-item nodeHeader" data-toggle="collapse"><h5>Receiver ' + cnt + ' Id: HyJbibDe-</h5></a><div class="collapse nodeItem" id= "demo' + cnt + '" ><a href="#" class="list-group-item">Subitem 1</a><a href="#" class="list-group-item">Subitem 2</a><a href="#" class="list-group-item">Subitem 3</a></div ></div >';
        $(receiverBlock).appendTo("#ReceiverContainer");
        
        var myReceiver = new SpeckleReceiver({
            restEndpoint: 'https://server.speckle.works', // or your own dist
            token: 'faa6ba741e854d6e8fe6ae66218ac736',
            streamId: 'HyJbibDe-'
        });

        myReceiver.on('ready', (name, layers, objects) => {
            console.log('ready');
            console.log(name);
            console.log(layers);
            console.log(objects);
        });

        myReceiver.on('live-update', (name, layers, objects, history) => {
            console.log('live-update');
            console.log(name);
            console.log(layers);
            console.log(objects);
            console.log(history);
        });

        myReceiver.on('metadata-update', (name, layers) => {
            console.log('metadata-update');
            console.log(name);
            console.log(layers);
        });

        receivers.push(myReceiver);
        
        cnt++;
    });

});