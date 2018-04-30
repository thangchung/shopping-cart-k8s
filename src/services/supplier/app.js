var express = require('express');
var app = express();

app.get('/', function(req, res) {
  var suppliers = [
    {
      id: 1,
      companyName: 'Company A',
      contactName: 'Contact A',
      contactTitle: 'Sale Manager'
    },
    {
      id: 2,
      companyName: 'Company B',
      contactName: 'Contact B',
      contactTitle: 'Secretary'
    }
  ];
  res.send(suppliers);
});

app.get('/health', function(req, res) {
  res.send({ status: 'Supplier Service is healthy.' });
});

app.listen(3000, () => {
  console.log('App is running at http://localhost:3000');
  console.log('Press CTRL-C to stop\n');
});

module.exports = app;
