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
    group('Create Project', function () {
        const projectPayload = JSON.stringify({
            Title: `Project ${Math.random().toString(36).substr(2, 9)}`,
            ManagerId: '035b76fb-2e04-4c49-9b47-e2de220841e8', // Replace with a valid manager ID
            Description: 'Description of the project',
            Details: 'Details about the project',
        });

        const res = http.post(`${BASE_URL}/Project/Create`, projectPayload, {
            headers: { 'Content-Type': 'application/json' },
        });

        check(res, {
            'project created successfully': (r) => r.status === 302, // Expecting a redirect on success
        });
    });

    group('View Project Details', function () {
        const projectId = 6; // Use a valid project ID from your database

        const res = http.get(`${BASE_URL}/Project/Detail?id=${projectId}`);

        check(res, {
            'project details fetched': (r) => r.status === 200,
        });
    });

    sleep(1);
}
