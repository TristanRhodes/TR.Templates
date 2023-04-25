'use strict';

// https://github.com/postmanlabs/OpenAPI-to-Postman
const fs = require('fs');
const converter = require('openapi-to-postmanv2');

var files = fs.readdirSync('/artifacts/swagger');

files.forEach(file => {
    console.log(file);

    var postmanFile = file.replace('swagger', 'collection');
    const openapiData = fs.readFileSync(`/artifacts/swagger/${file}`, { encoding: 'UTF8' });

    converter.convert({ type: 'string', data: openapiData },
        {}, (err, conversionResult) => {

            if (!conversionResult.result) {
                throw err;
            }

            var payload = conversionResult.output[0].data;
            console.log('The collection object is: ', payload);

            fs.mkdir('/artifacts/postman/', { recursive: true }, (e) => {
                if (e)
                    throw e;
            });
            fs.writeFileSync(`/artifacts/postman/${postmanFile}`, JSON.stringify(payload));

        }
    );

});

const openapiData = fs.readFileSync('/artifacts/swagger/Template.DbApi.Api.swagger.json', { encoding: 'UTF8' });

converter.convert({ type: 'string', data: openapiData },
    {}, (err, conversionResult) => {

        if (!conversionResult.result) {
            throw err;
        }

        var payload = conversionResult.output[0].data;
        console.log('The collection object is: ', payload);

        fs.mkdir('/artifacts/postman/', { recursive: true }, (e) => {
            if (e)
                throw e;
        });
        fs.writeFileSync('/artifacts/postman/Template.DbApi.Api.collection.json', JSON.stringify(payload));
        
    }
);


const express = require('express');

// Constants
const PORT = 8080;
const HOST = '0.0.0.0';

// App
const app = express();
app.get('/', (req, res) => {
  res.send('Hello World');
});

app.listen(PORT, HOST, () => {
  console.log(`Running on http://${HOST}:${PORT}`);
});

// docker build . -t tr/openapi-to-postmanv2
// set directory to app root

// From Root: docker build ./docker/OpenApiToPostman/ -t tr/openapi-to-postmanv2
// From Root: docker run -d -v C:\Git\Template.TestedLibrary\templates\Template.DbApi\artifacts:/artifacts -p 8080:8080 tr/openapi-to-postmanv2

// Volumes need full path - need to use docker compose to get relative paths.