
const checkUICompatibility = () => {
    if(!('serviceWorker' in navigator)) {
        console.log('Service worker not supported');
       throw new Error('Service worker not supported');
    }

    if(!('Notification' in window)) {
        console.log('Notification not supported');
        throw new Error('Notification not supported');
    }
}

const displayNotificationButtons = () => {

    const enableNotificationsButtons = document.querySelectorAll('.enable-notifications');

    for (const element of enableNotificationsButtons) {
        if (Notification.permission === 'default') {
            element.style.display = 'inline-block';
        }
        else { element.style.display = 'none'; }
    }
}

//step 1: check if the browser supports service workers and notifications
checkUICompatibility();

//step 2: manage ui elements based on existing permission status
displayNotificationButtons();

//step 3: register the service worker
let registration = navigator.serviceWorker.register('service-worker.js');
let thisEndpoint = '';
let initialPermission = '';

//step 4: get subscription if exists
requestSubscription();

async function requestSubscription() {
    //add check for referrer
    const worker = await navigator.serviceWorker.getRegistration();
    const existingSubscription = await worker.pushManager.getSubscription();
    
    if (existingSubscription) {
        thisEndpoint = existingSubscription.endpoint;
        sendSubscriptionToServer(existingSubscription, true);
    }
}

async function requestNotificationPermission() {
    await Notification.requestPermission();
}

function registerPush(registration) {
    const applicationServerPublicKey = window.pushNotificationKey;
    const applicationServerKey = urlB64ToUint8Array(applicationServerPublicKey);
    
    return registration.pushManager.subscribe({
        userVisibleOnly: true,
        applicationServerKey: applicationServerKey
    })
        .then(subscription => {
            thisEndpoint = subscription.endpoint;
            return sendSubscriptionToServer(subscription, false);
        })
        .catch(error => {
            console.error('Failed to subscribe the user: ', error);
        });
}

function sendSubscriptionToServer(subscription, isSubscribed) {

    const requestData = new SubscriptionRequest(
        subscription.endpoint,
        arrayBufferToBase64(subscription.getKey('p256dh')),
        arrayBufferToBase64(subscription.getKey('auth')),
        isSubscribed
    );

    return fetch('/Profile/AddSubscription', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestData)

    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Error from server while adding subscription.');
            }
            displayNotificationButtons();
        })
        .catch(error => {
            console.error('Error sending subscription to server:', error);
        });
}

async function unsubscribe() {
    const requestData = thisEndpoint;
        return fetch('/Profile/RemoveSubscription', {
            method: 'DELETE',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(requestData)
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Error from server while removing subscription.');
                }
                displayNotificationButtons();
            })
            .catch(error => {
                console.error('Error sending subscription to server:', error);
            });
}

function urlB64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding)
        .replace(/-/g, '+')
        .replace(/_/g, '/');

    const rawData = window.atob(base64);
    const outputArray = new Uint8Array(rawData.length);

    for (let i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
}

function arrayBufferToBase64(buffer) {
    // https://stackoverflow.com/a/9458996
    var binary = '';
    var bytes = new Uint8Array(buffer);
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
};

class SubscriptionRequest {
    constructor(endpoint, publicKey, authenticationsecret, issubscribed) {
        this.endpoint = endpoint;
        this.publicKey = publicKey;
        this.authenticationsecret = authenticationsecret;
        this.issubscribed = issubscribed;
    }
}

async function handlePermissionChange(permission) {
    if (permission === 'granted') {
        console.log("Notifications changed to enabled by the user.");
        const worker = await navigator.serviceWorker.getRegistration();
        if (!worker) {
            requestNotificationPermission();
        }
        else {
            registerPush(worker)
        };
    } else if (permission === 'denied') {
        console.log("Notifications changed to disabled by the user.");
        unsubscribe();
    }
}

document.addEventListener('DOMContentLoaded', event => {
    navigator.permissions
        .query({ name: "notifications" })
        .then((permissionStatus) => {
            permissionStatus.onchange = () =>  {
                    handlePermissionChange(permissionStatus.state);
                        }
        });
});