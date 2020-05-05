(function () {
    $('.vote-btn').on('click', CheckNextTurn);

    function CheckNextTurn(e) {
        var data = { gameId: $(e.target).data('gameid') };

        $.ajax({
            url: `/Game/Play/CheckNextTurn`,
            type: "post",
            contentType: 'application/x-www-form-urlencoded',
            data: data,
            success: function (data) {
                if (data.success) {
                    if (data.nextTurn) {
                        NextTurn($(e.target).data('gameid'));
                    } else {
                        $('.vote-btn').addClass('disabled');
                        $('.vote-btn').text('...Waiting For Votes');
                    }
                }
            }
        });
    };

    function NextTurn(gameId) {
        var data = { gameId: gameId };

        $.ajax({
            url: `/Game/Play/NextTurn`,
            type: "post",
            contentType: 'application/x-www-form-urlencoded',
            data: data,
            success: function (data) {
                if (data.success) {
                    window.location.href = data.url;
                }
            }
        });
    };
})();