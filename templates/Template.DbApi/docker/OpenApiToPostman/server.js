'use strict';

// https://github.com/postmanlabs/OpenAPI-to-Postman
const fs = require('fs');
const converter = require('openapi-to-postmanv2');

var files = fs.readdirSync('/swagger');

files.forEach(file => {
    console.log(file);

    var postmanFile = file.replace('swagger', 'collection');
    const openapiData = fs.readFileSync(`/swagger/${file}`, { encoding: 'UTF8' });

    converter.convert({ type: 'string', data: openapiData },
        {}, (err, conversionResult) => {

            if (!conversionResult.result) {
                throw err;
            }

            var payload = conversionResult.output[0].data;
            console.log('The collection object is: ', payload);

            fs.writeFileSync(`/postman/${postmanFile}`, JSON.stringify(payload));
        }
    );

});

// docker build . -t tr/openapi-to-postmanv2
// set directory to app root

// From Root: docker build ./docker/OpenApiToPostman/ -t tr/openapi-to-postmanv2
// From Root: docker run -d -v C:\Git\Template.TestedLibrary\templates\Template.DbApi\artifacts:/artifacts -p 8080:8080 tr/openapi-to-postmanv2


// From Root: docker run -d -v C:\Git\Template.TestedLibrary\templates\Template.DbApi\artifacts\swagger:/swagger -v C:\Git\Template.TestedLibrary\templates\Template.DbApi\artifacts\postman:/postman -p 8080:8080 tr/openapi-to-postmanv2

// Volumes need full path - need to use docker compose to get relative paths.