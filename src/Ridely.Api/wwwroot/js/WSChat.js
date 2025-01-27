var userToken = null;
var userData = null;
//Recipient = { "firstname": null, "lastname": null, "phone": "\u002B2345678902", "profileImage": null, "id": "0e16991c-cc1e-4894-926c-01ccaba19d5e" };
var loading = true;
var Connected = false;
var socket = null;
var inputBox = document.getElementById("MessageInput")
var ChatPreview = document.getElementById("messages")


function sendMessage() {
    if (Connected) {
        socket.send(JSON.stringify({
            "EventName": "Trip.SendChatMessage",
            "EventArgs": {
                "Recipient": Recipient.id,
                "Message": inputBox.value
            }
        }));
        AddMessage(inputBox.value, userData.id)
        
    }
    
}
function AddMessage(message ,SenderID, Image) {
    if (message) {
        if (MessageRecord.length == 0) {
            MessageRecord.push(
                {
                    "uid": SenderID,
                    "message": [message],
                    "Image": Image,
                }
            )
        } else {
            if (MessageRecord[MessageRecord.length - 1].uid == SenderID) {
                MessageRecord[MessageRecord.length - 1].message.push(message)
                console.log("Adding")
            } else {
                MessageRecord.push(
                    {
                        "uid": SenderID,
                        "message": [message],
                    }
                )
            }
        }


        inputBox.value = ""
        RePopulate()

    }
}


function RePopulate() {
    ChatPreview.innerHTML = "";
    MessageRecord.forEach(element => {
        ChatPreview.innerHTML += GenerateBubble(element.message, element.uid == userData.id,element.Image )
    })
}

function GenerateBubble(Message, Left = true, Image = "") {

    var page = ""
    var bubbles = ""
    Message.forEach(element => {
        if (Left) {
            bubbles += `<p class="small p-2 ms-3 mb-1 rounded-3 bg-body-tertiary">${element}</p>`
        } else {
            bubbles += `<p class="small p-2 ms-3 mb-1 rounded-3 bg-primary">${element}</p>`
        }
        
    })
    if (!Image) {
        Image = "https://img.freepik.com/premium-vector/man-avatar-profile-picture-vector-illustration_268834-538.jpg"
    }
    if (Left) {
        page = `
        <div class="d-flex flex-row justify-content-start">
            <img class="profileBubble" src="${Image}"
                    alt="avatar 1" style="width: 45px; height: 100%;">
            <div>
                ${bubbles}
            </div>
        </div>
        `
    } else {
        page = `
        <div class="d-flex flex-row justify-content-end mb-4 pt-1">
            <div>
                ${bubbles}
            </div>
            <img class="profileBubble" src="${Image}"
                    alt="avatar 1" style="width: 45px; height: 100%;">
        </div>
        `
    }
    return page
}

function SetUser(jwt) {
    userToken = jwt;
    setCookie("Authorization", "Bearer " + jwt, 2)
    ConnectToWS();
}



function ConnectToWS() {
    // create websocket instance uid += "/web-chat";
    socket = new WebSocket("wss://api.ridely.app/web-chat");
    // add event listener reacting when message is received
    socket.onmessage = function (event) {
        // put text into our output div
        RCV(event)
    };
    
    
    socket.onclose = function (event) {

        socket = null;
        Connected =false
        setTimeout(ConnectToWS, 3000)
        StartLoading()
        
    };
    socket.onerror = function (event) {
        console.error(event)
    };

    socket.onopen = function () {
        
        fetch("/api/Passanger/Get",{
            method: "GET",
            headers: {
                "Authorization": "Bearer " + userToken
            }
        })
            .then(response => response.json())
            .then((userdata) => {
                if (userdata) {
                    userData = userdata
                    Connected = true
                    StopLoading()
                    OldMessages.forEach((item) => {

                        var image = item.image
                        if (image == null) {
                            image = "https://img.freepik.com/premium-vector/man-avatar-profile-picture-vector-illustration_268834-538.jpg"
                        }
                        AddMessage(item.message, item.uid, image)
                    })
                    RePopulate()
                }
                
            })
        
    };

}

function RCV(event) {
    console.log(event)
    var data = JSON.parse(event.data);
    if (data.Event == "NewMessage") {
        AddMessage(data.Data.Message, Recipient.id, Recipient.profileImage)
    }
        
}
function StopLoading() {
    loading = false;
    document.querySelector(".Loader").classList.remove("show")

}

function StartLoading() {
    loading = true;
    document.querySelector(".Loader").classList.add("show")
}

function setCookie(cname, cvalue, exdays) {
    const d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + encodeURIComponent(cvalue) + ";" + expires + ";path=/";
}

