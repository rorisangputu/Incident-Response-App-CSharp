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
    const projectId = 3; // Use a valid project ID from your database

    group('Add Project Task', function () {
        const taskPayload = JSON.stringify({
            Title: `Task ${Math.random().toString(36).substr(2, 9)}`,
            ProjectId: projectId,
            AssignedAt: new Date().toISOString(),
            CompletedAt: null,
        });

        const res = http.post(`${BASE_URL}/ProjectTask/AddTask`, taskPayload, {
            headers: { 'Content-Type': 'application/json' },
        });

        check(res, {
            'task added successfully': (r) => r.status === 302, // Expecting a redirect on success
        });
    });

    sleep(1);
}
