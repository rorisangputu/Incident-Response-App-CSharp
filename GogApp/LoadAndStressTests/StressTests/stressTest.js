import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
  stages: [
    { duration: '1m', target: 50 },    // Ramp up to 50 users over 1 minute
    { duration: '2m', target: 200 },   // Ramp up to 200 users over 2 minutes
    { duration: '3m', target: 500 },   // Ramp up to 500 users over 3 minutes
    { duration: '2m', target: 500 },   // Hold at 500 users for 2 minutes
    { duration: '2m', target: 0 },     // Ramp down to 0 users over 2 minutes
  ],
};

const BASE_URL = 'http://localhost:5014';  // Replace with your actual API base URL

export default function () {
  // HomeController - Load home page
  let homeRes = http.get(`${BASE_URL}/`);
  check(homeRes, {
    'home page loaded successfully': (r) => r.status === 200,
  });

  // HomeController - Load privacy page
  let privacyRes = http.get(`${BASE_URL}/Home/Privacy`);
  check(privacyRes, {
    'privacy page loaded successfully': (r) => r.status === 200,
  });

  // AccountController - Register a new user
  let registerRes = http.post(`${BASE_URL}/Account/Register`, JSON.stringify({
    EmailAddress: `testuser${Math.floor(Math.random() * 10000)}@example.com`,
    Name: `TestUser${Math.floor(Math.random() * 10000)}`,
    Password: 'password123',
  }), { headers: { 'Content-Type': 'application/json' } });
  check(registerRes, {
    'user registered successfully': (r) => r.status === 302,  // assuming a redirect on success
  });

  // AccountController - Log in with registered user (you may want to provide a fixed user for consistency)
  let loginRes = http.post(`${BASE_URL}/Account/Login`, JSON.stringify({
    EmailAddress: 'rorisangputu@gmail.com',  // use a predefined user for testing
    Password: 'Rorisang@197',
  }), { headers: { 'Content-Type': 'application/json' } });
  check(loginRes, {
    'user logged in successfully': (r) => r.status === 302,  // assuming a redirect on success
  });

  // ProjectController - Create a new project
  let createProjectRes = http.post(`${BASE_URL}/Project/Create`, JSON.stringify({
    Title: `Test Project ${Math.floor(Math.random() * 10000)}`,
    Description: 'This is a test project created during stress testing.',
    Details: 'Detailed description for the test project.',
  }), { headers: { 'Content-Type': 'application/json' } });
  check(createProjectRes, {
    'project created successfully': (r) => r.status === 302,  // assuming a redirect on success
  });

  // ProjectController - Retrieve list of projects
  let projectsRes = http.get(`${BASE_URL}/Project/Index`);
  check(projectsRes, {
    'projects loaded successfully': (r) => r.status === 200,
  });

  // ProjectController - Get project details (using a fixed project ID for simplicity)
  let projectDetailRes = http.get(`${BASE_URL}/Project/Detail?id=1`);
  check(projectDetailRes, {
    'project detail loaded successfully': (r) => r.status === 200,
  });

  // ProjectController - Delete project (assuming deletion of a specific project ID for testing)
  let deleteProjectRes = http.post(`${BASE_URL}/Project/Delete/3`, null, { headers: { 'Content-Type': 'application/json' } });
  check(deleteProjectRes, {
    'project deleted successfully': (r) => r.status === 302,  // assuming a redirect on success
  });

  // Pause to simulate realistic user behavior
  sleep(1);
}
