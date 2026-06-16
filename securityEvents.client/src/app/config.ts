// config.ts

export const API_BASE =
  location.hostname === 'localhost'
    ? 'https://localhost:60217'   // your local API
    : 'https://api.security.shufersal.co.il';
