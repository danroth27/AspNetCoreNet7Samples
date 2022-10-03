"use strict";

let b1 = document.getElementById("b1");
let b2 = document.getElementById("b2");
let b3 = document.getElementById("b3");
let b4 = document.getElementById("b4");
let b5 = document.getElementById("b5");
let b6 = document.getElementById("b6");
let b7 = document.getElementById("b7");
let b8 = document.getElementById("b8");
let b9 = document.getElementById("b9");
let buttons = [b1, b2, b3, b4, b5, b6, b7, b8, b9];
disableButtons();

var connection = new signalR.HubConnectionBuilder().withUrl("/game").build();
let playerId = 0;
connection.on("PlayerRegistered", function (id) {
    if (document.getElementById("join").disabled) {
        if (playerId === 0) {
            playerId = id;
            document.getElementById("playerId").innerHTML = `You are ${id === 1 ? "X" : "O"}`;
        }
        if (id === 1) {
            document.getElementById("playerTurn").innerHTML = "waiting for another player";
        } else {
            document.getElementById("playerTurn").innerHTML = "other players turn";
        }
    }
});

let promise;


connection.on("Win", function (winId) {
    disableButtons();
    if (winId === playerId) {
        alert("you win");
    } else {
        alert("other player won");
    }
});

connection.on("Disable", function (square) {
    buttons[square - 1].disabled = true;
    if (buttons[square - 1].value === '') {
        buttons[square - 1].value = playerId === 1 ? 'O' : 'X';
    }
});

connection.start().then(function () {
}).catch(function (err) {
    return console.error(err.toString());
});

let promiseResolve;

function disableButtons() {
    for (let i = 0; i < 9; i++) {
        buttons[i].disabled = true;
        buttons[i].removeEventListener("click", onClick);
    }
}

connection.on("Turn", function () {
    document.getElementById("playerTurn").innerHTML = "your turn";
    enableButtons();
    return promise;
});

function enableButtons() {
    promise = new Promise(function (resolve, reject) {
        promiseResolve = resolve;
    });
    for (let i = 0; i < 9; i++) {
        if (buttons[i].disabled && buttons[i].value === '') {
            buttons[i].disabled = false;
            buttons[i].addEventListener("click", onClick);
        }
    }
}

function onClick(element) {
    document.getElementById("playerTurn").innerHTML = "other players turn";
    element.target.value = playerId === 1 ? 'X' : 'O';
    disableButtons();
    promiseResolve(Number(element.target.id.split('b')[1]));
}

document.getElementById("join").addEventListener("click", function (event) {
    document.getElementById("join").disabled = true;
    connection.send("Join");
});