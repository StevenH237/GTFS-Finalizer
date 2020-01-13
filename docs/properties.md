This document is a summarizing explanation of how properties in the OSM file translate to tags in the GTFS file.

# agency.txt
One `type=network`, of which at least one `type=route_master` is a member, becomes one record in this file.

|Property|Explanation|
|:-|:-|
|`agency_name`|The value of the relation's `name` tag, if present; otherwise, the value of the relation's `name:xx` tag, where `xx` is a language code; otherwise, an exception is thrown.|
|`agency_id`|The value of the relation's `network` tag, if present; otherwise, the acronym of the `agency_name` value above. Skipped if only one agency is present.|
|`agency_url`|The value of the relation's `website` tag, if present; otherwise, the value of the relation's `contact:website` tag, if present; otherwise, an exception is thrown.|
|`agency_timezone`|Prompted for within the program, since it must be the same for all agencies in the file.|
|`agency_lang`|Is always skipped. A language can be specified via `feed_lang` in the `gtfs/feed_info.txt` file.|
|`agency_phone`|The value of the relation's `phone` tag, if present; otherwise, the value of the relation's `contact:phone` tag, if present; otherwise, this value is skipped.|
|`agency_fare_url`|The value of the relation's `website:fares` tag, if present; otherwise, the value of the relation's `contact:website:fares` tag, if present; otherwise, this value is skipped.|
|`agency_email`|The value of the relation's `email` tag, if present; otherwise, the value of the relation's `contact:email` tag, if present; otherwise, this value is skipped.|