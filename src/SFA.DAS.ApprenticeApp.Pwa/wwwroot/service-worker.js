
self.addEventListener('fetch', event => { return null; });

self.addEventListener('push', event => {
    const payload = event.data.json();
    event.waitUntil(
        self.registration.showNotification(payload.title, {
            body: payload.message,
            icon: '/assets/images/YourApprenticeship.svg',
            vibrate: [100, 50, 100],
            data: { url: payload.url }
        })
    );
});

self.addEventListener('notificationclick', event => {
    event.notification.close();
    event.waitUntil(clients.openWindow(event.notification.data.url));
});

