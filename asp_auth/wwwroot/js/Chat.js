"use strict";

let debugCredentials = [
    { username: "userone@email.com", password: "password" },
    { username: "usertwo@email.com", password: "password" },
];
let baseUrl = `${window.location.protocol}//${window.location.host}/`;
let authUrl = new URL("api/User/Authenticate", baseUrl);

async function postData(url = '', data = {}) {
    const response = await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
    });
    return await response.json();
};

async function getToken() {
    const userOne = await postData(authUrl, debugCredentials[0]);
    const userTwo = await postData(authUrl, debugCredentials[1]);

    let tokens = [
        userOne.token,
        userTwo.token
    ];
    let ids = [
        userOne.id,
        userTwo.id
    ];

    const randomNum = Math.random();
    ids = randomNum < 0.5 ? [ids[0], ids[1]] : [ids[1], ids[0]];
    tokens = randomNum < 0.5 ? [tokens[0], tokens[1]] : [tokens[1], tokens[0]];

    document.getElementById("senderInput").value = ids[0];
    document.getElementById("receiverInput").value = ids[1];

    return tokens[0];
}

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/Messages/Hub", {
        accessTokenFactory: async () => {
            return await getToken();
        }
    })
    .build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (messageID, displayName, message) {
    const msg = message
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
    const encodedMsg = displayName + " says " + msg;

    const li = document.createElement("li");
    const lispan = document.createElement("span");
    const button = document.createElement("button");
    const buttonspan = document.createElement("span");

    addEventToButton(button);

    lispan.textContent = encodedMsg;
    li.className = "list-group-item";
    li.id = messageID;
    button.type = "button";
    button.className = "close";
    button.id = "deleteButton";
    buttonspan.innerHTML = "&times;";

    button.appendChild(buttonspan);
    li.appendChild(lispan);
    li.appendChild(button);

    document.getElementById("messagesList").appendChild(li);
});

connection.on("DeleteSuccess", function (messageId = "") {
    const message = document.getElementById(messageId);
    if (message) {
        message.parentNode.removeChild(message);
    }
});

connection
    .start()
    .then(function () {
        document.getElementById("sendButton").disabled = false;
    })
    .catch(function (err) {
        return console.error(err.toString());
    });

document
    .getElementById("sendButton")
    .addEventListener("click", function (event) {
        const sender = document.getElementById("senderInput").value;
        const receiver = document.getElementById("receiverInput").value;
        const input = document.getElementById("messageInput").value;
        const message = {
            SenderId: sender,
            ReceiverId: receiver,
            MessageContent: input
        };
        connection.invoke("SendMessage", message).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });

const listItems = document.querySelectorAll("#deleteButton").forEach(item => {
    addEventToButton(item);
});

function addEventToButton(button) {
    button.addEventListener("click", event => {
        const messageID = event.path[2].id || null;
        // const sender = document.getElementById("senderInput").value;

        connection.invoke("DeleteMessage", messageID).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
        event.preventDefault();
    });
}
