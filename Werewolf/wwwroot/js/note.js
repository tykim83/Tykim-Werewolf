(function () {
    //Setup Event Listener on textarea
    $('.note-text').on('change', getValueFromTheNote);

    //Stop Editing textarea on Enter -- this will trigger the Save
    $('.note-text').on('keypress', function (e) {
        if (e.keyCode === 13 || e.which === 13) {
            $($(e.target)).blur();
        };
    });

    //Get values on change to textarea
    function getValueFromTheNote(e) {
        let id, gameId, opponentId, message;
        id = e.target.id;
        opponentId = $(e.target).data('opponentid');
        gameId = $(e.target).data('gameid');
        message = $(e.target).val();

        if (id === "") {
            //Add New Note
            AddNote(e.target, gameId, opponentId, message);
        } else {
            //Update Note
            UpdateNote(id, message);
        }
    };

    function AddNote(el, gameId, opponentId, message) {
        var data = { gameId: gameId, opponentId: opponentId, message: message };

        $.ajax({
            //url: '@Url.Action("AddNote", "Play")',
            url: `/Game/Play/AddNote`,
            type: "post",
            contentType: 'application/x-www-form-urlencoded',
            data: data,
            success: function (result) {
                //Add new Id to the element
                el.id = result.id;
                if (result.success) {
                    toastr.success(result.message);
                } else {
                    toastr.error(result.message);
                }
            }
        });
    };

    function UpdateNote(id, message) {
        var data = { id: id, message: message };

        $.ajax({
            url: `/Game/Play/UpdateNote`,
            type: "post",
            contentType: 'application/x-www-form-urlencoded',
            data: data,
            success: function (data) {
                console.log(data.message);
                if (data.success) {
                    toastr.success(data.message);
                } else {
                    toastr.error(data.message);
                }
            }
        });
    };

})();