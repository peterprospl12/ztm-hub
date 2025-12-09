import { test, expect } from '@playwright/test';

async function mockAuth(page: any) {
    const fakeToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InRlc3RAZXhhbXBsZS5jb20iLCJleHAiOjk5OTk5OTk5OTl9.fake';
    await page.goto('/');
    await page.evaluate((token: string) => localStorage.setItem('token', token), fakeToken);
}

test('should navigate between login and register pages', async ({ page }) => {
    await page.goto('/login');
    await page.getByRole('link', { name: /sign up/i }).click();

    await expect(page).toHaveURL('/register');
    await expect(page.getByRole('heading', { name: /create account/i })).toBeVisible();

    await page.getByRole('link', { name: /log in/i }).click();
    await expect(page).toHaveURL('/login');
});

test('should redirect to login when accessing dashboard without auth', async ({ page }) => {
    await page.goto('/');
    await page.evaluate(() => localStorage.clear());
    await page.goto('/dashboard');

    await expect(page).toHaveURL('/login');
});

test('should display dashboard with all main sections', async ({ page }) => {
    await mockAuth(page);
    await page.goto('/dashboard');

    await expect(page.getByRole('heading', { name: /ztm hub/i })).toBeVisible();
    await expect(page.getByRole('heading', { name: /your stops/i })).toBeVisible();
    await expect(page.getByRole('heading', { name: /map/i })).toBeVisible();
    await expect(page.getByRole('heading', { name: /departures/i })).toBeVisible();
    await expect(page.getByRole('button', { name: /logout/i })).toBeVisible();
});

test('should toggle map mode between favorites and all stops', async ({ page }) => {
    await mockAuth(page);
    await page.goto('/dashboard');

    const myStopsBtn = page.getByRole('button', { name: /my stops/i });
    const allStopsBtn = page.getByRole('button', { name: /all stops/i });

    await expect(myStopsBtn).toHaveClass(/bg-blue-600/);

    await allStopsBtn.click();
    await expect(allStopsBtn).toHaveClass(/bg-blue-600/);
});

test('should logout and redirect to login', async ({ page }) => {
    await mockAuth(page);
    await page.goto('/dashboard');

    await page.getByRole('button', { name: /logout/i }).click();

    await expect(page).toHaveURL('/login');
    const token = await page.evaluate(() => localStorage.getItem('token'));
    expect(token).toBeNull();
});