(function () {
    $('select').on('change', getValueFromTheVote);

    function getValueFromTheVote(e) {
        let id, gameId, role, userVoteId, currentUserVoteId;
        id = e.target.id;
        gameId = $(e.target).data('gameid');
        role = $(e.target).data('role');
        currentUserVoteId = $(e.target).data('currentvoteuserid');
        userVoteId = $(e.target).val();

        $(e.target).blur();

        if (id == 0) {
            //Add New Vote
            AddVote(e.target, gameId, role, userVoteId);
        } else {
            //Update Vote
            UpdateVote(e.target, id, userVoteId);
        }
    };

    function AddVote(el, gameId, role, userVoteId) {
        var data = { gameId: gameId, role: role, userVoteId: userVoteId };

        $.ajax({
            url: `/Game/Play/AddVote`,
            type: "post",
            contentType: 'application/x-www-form-urlencoded',
            data: data,
            success: function (result) {

                if (result.success) {
                    toastr.success(result.message);

                    //Add new Id to the element DOM
                    el.id = result.id;

                    //Add currentUserVoteId to the element Data
                    $(el).attr('data-currentvoteuserid', userVoteId);

                    //Add Vote to the DOM
                    AddVoteToDOM(userVoteId);
                } else {
                    toastr.error(result.message);
                }
            }
        });
    };

    function UpdateVote(el, id, userVoteId) {
        var data = { id: id, userVoteId: userVoteId };

        $.ajax({
            url: `/Game/Play/UpdateVote`,
            type: "post",
            contentType: 'application/x-www-form-urlencoded',
            data: data,
            success: function (data) {

                if (data.success) {
                    toastr.success(data.message);

                    //Remove the previous vote
                    RemoveVoteFromDOM($(el).attr('data-currentvoteuserid'));

                    //change the DOM with the new Id
                    $(el).attr('data-currentvoteuserid', userVoteId);

                    //Add Vote to the DOM
                    AddVoteToDOM(userVoteId);

                } else {
                    toastr.error(data.message);
                }
            }
        });
    };

    function AddVoteToDOM(vote) {
        var current = $('#voteReceived-' + vote).text();
        $('#voteReceived-' + vote).text(parseInt(current) + 1);
    };

    function RemoveVoteFromDOM(vote) {
        var current = $('#voteReceived-' + vote).text();
        $('#voteReceived-' + vote).text(parseInt(current) - 1);
    };

})();