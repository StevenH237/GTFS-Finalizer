This document is a summarizing explanation of how properties in the OSM file translate to tags in the GTFS file.

# agency.txt
One `type=network`, of which at least one `type=route_master` is a member, becomes one record in this file.

|Property|Explanation|
|:-|:-|
|`agency_id`|The relation's ID, unless `IDs.Agency` is specified in `settings.json` - in which case, the value of the tag specified by `ids.agency`.|
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

# routes.txt
One `type=route`, `route=monorail|tram|subway|train|bus|trolleybus|ferry|aerialway` becomes one record in this file.

|Property|Explanation|
|:-|:-|
|`route_id`|Usually the ID of the openstreetmap relation. However, if `IDs.Route` is specified in `settings.json`, it specifies the tag to use as an ID.|
|`agency_id`|The `type=network` relation that contains the route. See below this table for handling cases of multiple membership.|
|`route_short_name`|The `ref` tag of the route_master relation.|
|`route_long_name`|By default, the `name` tag of the route_master relation. However, if `Names.Route` is specified in `settings.json`, and has a value other than empty string, the tag specified dictates the name of the route. If the value is empty string, every route will prompt for a name via the console.|
|`route_type`|Based on the `route_master` tag of the relation - `monorail` or `tram` becomes 0; `subway` 1; `train` 2; `bus` or `trolleybus` 3; `ferry` 4; `aerialway` 6. There's no equivalent to 5 (cable car) or 7 (funicular).|
|`route_desc`|The `desc` tag of the route_master relation.|
|`route_url`|The `website` or `contact:website` tags of the relation.|
|`route_color`|The `colour` or `color` tags of the relation.|
|`route_text_color`|Either black or white, whichever is farther in brightness from the `route_color`. If the `route_color` is unspecified, so is the `route_text_color`.|
|`route_sort_order`|The order of the route within its network relation.|

## Multiple membership
If the route is part of multiple networks, the network to which the route is assigned is chosen as follows:

* If the route has the `operates` role in all or none of those networks, the network of which the route is a part is chosen arbitrarily.
* If the route has the `operates` role in some, but not all, of those networks, the network of which the route is a part is chosen arbitrarily among those for which it does *not* have the role.
