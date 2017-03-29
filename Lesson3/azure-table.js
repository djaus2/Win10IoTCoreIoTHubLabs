/*
* IoT Hub Raspberry Pi NodeJS - Microsoft Sample Code - Copyright (c) 2016 - Licensed MIT
*/
'use strict';

var moment = require('moment');
var storage = require('azure-storage');
var stopReadAzureTable = false;

/**
 * Read messages from Azure Table.
 * @param {object}  config - config object
 */
var readAzureTable = function (config) {
  var tableService = storage.createTableService(config.azure_storage_connection_string);
  var timestamp = moment.utc().format('hhmmssSSS');

  var readNewMessages = function () {
    var tableName = 'DeviceData';
    var condition = 'PartitionKey eq ? and RowKey gt ? ';
    // Only query messages that're no later than the current time
    var query = new storage.TableQuery().where(condition, moment.utc().format('YYYYMMDD'), timestamp);
    tableService.queryEntities(tableName, query, null, function (error, result) {
      if (error) {
        if (error.statusCode && error.statusCode == 404) {
          console.error(
            '[Azure Table] ERROR: Table not found. Something might be wrong. Please go to troubleshooting page for more information.')
        } else {
          console.error('[Azure Table] ERROR:\n' + error);
        }
        readNewMessages();
        return;
      }

      // result.entries contains entities matching the query
      if (result.entries.length > 0) {
        for (var i = 0; i < result.entries.length; i++) {
          console.log('[Azure Table] Read message: ' + result.entries[i].message['_'] + '\n');

          // Update timestamp so that we don't get old messages
          if (result.entries[i].RowKey['_'] > timestamp) {
            timestamp = result.entries[i].RowKey['_'];
          }
        }
      }
      if (!stopReadAzureTable) {
        readNewMessages();
      }
    });
  }

  readNewMessages();
}

/**
 * Set stopReadAzureTable flag to true.
 */
var cleanup = function () {
  stopReadAzureTable = true;
}

module.exports.readAzureTable = readAzureTable;
module.exports.cleanup = cleanup;
