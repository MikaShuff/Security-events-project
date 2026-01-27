
// securityevents.client/proxy.conf.js

module.exports = [
  {
    context: ['/api'],
    target: 'http://localhost:5000',  // API שלך רץ על HTTP:5000
    secure: false,
    changeOrigin: true,
    logLevel: 'debug'
  }
];

