from azure.eventgrid import EventGridClient
from msrest.authentication import TopicCredentials
from datetime import datetime
import json
import os, uuid

class EventGridSinkWriter:
    def __init__(self, topic : str, key: str, event_type: str):
        self.eg_accesskey = key
        self.eg_topicurl = topic
        self.event_type = event_type
    
    def open(self, partition_id, epoch_id):
        # This is called first when preparing to send multiple rows.
        credentials = TopicCredentials(self.eg_accesskey)
        self.event_grid_client = EventGridClient(credentials)
        return True

    def process(self, row):
        # This is called for each row after open() has been called.
        print(row)
        self.publish_event(row)

    def close(self, err):
        # This is called after all the rows have been processed.
        if err:
            raise err
        
    def publish_event(self, row):
        subject = row['Subject']
        message_body = json.dumps(row.asDict())
        self.event_grid_client.publish_events(
            self.eg_topicurl,
            events=[{
                'id' : str(uuid.uuid4()),
                'subject' : subject,
                'data': message_body,
                'event_type': self.event_type,
                'event_time': datetime.utcnow(),
                'data_version': 1
            }]
        )