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

$(window).load(function () {

    var cnt = 0;

    $('#AddSender').click(function () {
        console.log('Adding Sender');
        var senderBlock = '<div class="sortableContainer"><a id="sortableNode" href="#demo' + cnt + '" class="list-group-item nodeHeader" data-toggle="collapse"><h5>Sender' + cnt + ' Id: xxxxx</h5></a><div class="collapse nodeItem" id= "demo' + cnt + '" ><a href="#" class="list-group-item">Subitem 1</a><a href="#" class="list-group-item">Subitem 2</a><a href="#" class="list-group-item">Subitem 3</a></div ></div >';
        $(senderBlock).appendTo("#SenderContainer");
        cnt++;
    });

    $('#AddReceiver').click(function () {
        console.log('Adding Receiver');
        var receiverBlock = '<div class="sortableContainer"><a id="sortableNode" href="#demo' + cnt + '" class="list-group-item nodeHeader" data-toggle="collapse"><h5>Receiver ' + cnt + ' Id: xxxxx</h5></a><div class="collapse nodeItem" id= "demo' + cnt + '" ><a href="#" class="list-group-item">Subitem 1</a><a href="#" class="list-group-item">Subitem 2</a><a href="#" class="list-group-item">Subitem 3</a></div ></div >';
        $(receiverBlock).appendTo("#ReceiverContainer");
        cnt++;
    });

});