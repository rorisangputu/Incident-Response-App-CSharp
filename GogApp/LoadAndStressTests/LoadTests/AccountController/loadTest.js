import http from 'k6/http';
import { check, group, sleep } from 'k6';

const BASE_URL = 'http://localhost:5014'; // Change this to your application URL

export let options = {
    stages: [
        { duration: '30s', target: 100 }, // Ramp up to 100 users over 30 seconds
        { duration: '1m', target: 100 },   // Stay at 100 users for 1 minute
        { duration: '30s', target: 0 },     // Ramp down to 0 users
    ],
};

export default function () {
    group('User Registration', function () {
        const registerPayload = JSON.stringify({
            Name: `user${Math.random().toString(36).substr(2, 9)}`,
            EmailAddress: `test${Math.random().toString(36).substr(2, 9)}@example.com`,
            Password: 'Password123!',
        });

        const res = http.post(`${BASE_URL}/Account/Register`, registerPayload, {
            headers: { 'Content-Type': 'application/json' },
        });

        check(res, {
            'registration successful': (r) => r.status === 302, // Expecting a redirect on success
        });
    });

    group('User Login', function () {
        const loginPayload = JSON.stringify({
            EmailAddress: 'test@example.com', // Use a valid email from your registration
            Password: 'Password123!',
        });

        const res = http.post(`${BASE_URL}/Account/Login`, loginPayload, {
            headers: { 'Content-Type': 'application/json' },
        });

        check(res, {
            'login successful': (r) => r.status === 302, // Expecting a redirect on success
        });
    });

    sleep(1);
}
