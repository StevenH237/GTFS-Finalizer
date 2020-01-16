This document is a summarizing explanation of how properties in the OSM file translate to tags in the GTFS file.

# agency.txt
One `type=network`, of which at least one `type=route_master` is a member, becomes one record in this file.

|Property|Explanation|
|:-|:-|
|`agency_id`|The relation's ID, unless `ids.agency` is specified in `settings.json` - in which case, the value of the tag specified by `ids.agency`.|
|`agency_name`|The value of the relation's `name` tag, if present; otherwise, the value of the relation's `name:xx` tag, where `xx` is a language code; otherwise, an exception is thrown.|
|`agency_url`|The value of the relation's `website` tag, if present; otherwise, the value of the relation's `contact:website` tag, if present; otherwise, an exception is thrown.|
|`agency_timezone`|Prompted for within the program, since it must be the same for all agencies in the file.|
|`agency_lang`|Is always skipped. A language can be specified via `feed_lang` in the `gtfs/feed_info.txt` file.|
|`agency_phone`|The value of the relation's `phone` tag, if present; otherwise, the value of the relation's `contact:phone` tag, if present; otherwise, this value is skipped.|
|`agency_fare_url`|The value of the relation's `website:fares` tag, if present; otherwise, the value of the relation's `contact:website:fares` tag, if present; otherwise, this value is skipped.|
|`agency_email`|The value of the relation's `email` tag, if present; otherwise, the value of the relation's `contact:email` tag, if present; otherwise, this value is skipped.|

# stops.txt
One `public_transport=platform` becomes one record in this file.

|Property|Explanation|
|:-|:-|
|`stop_id`|Usually the ID of the openstreetmap object, prefixed with `n`, `w`, or `r`.|
|`stop_name`|The value of the stop's `name` or `name:xx` tags, in order.|
|`stop_lat`|The latitude of the stop.|
|`stop_lon`|The longitude of the stop.|
|`location_type`|Only `0` is supported so far.|
|`stop_code`|The value of the stop's `ref` tag.|
|`stop_desc`|The value of the stop's `description` tag.|
|`zone_id`|The `zone` or `zone_id` tags of the stop.|
|`stop_url`|The `website` or `contact:website` tags of the stop.|
|`wheelchair_boarding`|`1` for `wheelchair=yes` or `wheelchair=limited` stops. `2` for `wheelchair=no` stops. Left empty otherwise.|