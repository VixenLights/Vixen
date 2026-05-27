# FPP REST API Reference

Falcon Player (FPP) v7.x exposes a REST API over HTTP. All endpoints are relative to `http://<fpp-host>/api/`.
Responses are JSON unless otherwise noted. The common response envelope for most endpoints includes:

```json
{ "Status": "OK", "Message": "", "respCode": 200 }
```

Path parameters are written as `:ParamName`. Optional path segments are shown in parentheses.

---

## fppd Endpoints

These endpoints interact with the running FPPD daemon process. All require FPPD to be running unless noted.

### GET `fppd/status`

Returns the full current status of the FPPD daemon.

**Response fields**

| Field | Type | Description |
|---|---|---|
| `fppd` | string | Daemon state: `"running"` or `"stopped"` |
| `status` | int | 0 = idle, 1 = playing |
| `status_name` | string | `"idle"` or `"playing"` |
| `mode` | int | Operating mode (2 = player) |
| `mode_name` | string | `"player"`, `"bridge"`, etc. |
| `current_playlist.playlist` | string | Name of the currently playing playlist |
| `current_playlist.type` | string | Entry type currently playing (e.g. `"pause"`, `"sequence"`) |
| `current_playlist.index` | string | 1-based index within the playlist |
| `current_playlist.count` | string | Total number of items in the playlist |
| `current_playlist.description` | string | Human-readable description of current entry |
| `current_sequence` | string | Filename of the currently playing sequence (empty if none) |
| `current_song` | string | Filename of the currently playing media (empty if none) |
| `seconds_elapsed` | string | Elapsed seconds for the current item |
| `seconds_remaining` | string | Remaining seconds for the current item |
| `seconds_played` | string | Total seconds played in this session |
| `time_elapsed` | string | Formatted elapsed time (`"MM:SS"`) |
| `time_remaining` | string | Formatted remaining time (`"MM:SS"`) |
| `repeat_mode` | int | 1 = repeat enabled |
| `volume` | int | Current output volume (0–100) |
| `uptime` | string | Formatted uptime (`"HH:MM"`) |
| `uptimeStr` | string | Human-readable uptime string |
| `uptimeTotalSeconds` | int | Total uptime in seconds |
| `multisync` | bool | Whether MultiSync is active |
| `bridging` | bool | Whether bridging mode is active |
| `scheduler.enabled` | int | 1 if scheduler is enabled |
| `scheduler.status` | string | `"playing"` or `"idle"` |
| `scheduler.currentPlaylist` | object | Current scheduler entry details |
| `scheduler.nextPlaylist` | object | Next scheduled playlist |
| `next_playlist.playlist` | string | Name of the next scheduled playlist |
| `next_playlist.start_time` | string | Human-readable next start time |
| `MQTT.configured` | bool | Whether MQTT is configured |
| `MQTT.connected` | bool | Whether MQTT is currently connected |
| `dateStr` | string | Current date string |
| `time` | string | Full date/time string |
| `timeStr` | string | Formatted time (`"HH:MM AM/PM"`) |
| `timeStrFull` | string | Formatted time with seconds |
| `uuid` | string | Unique device identifier |

**Example response (abbreviated)**

```json
{
  "fppd": "running",
  "status": 1,
  "status_name": "playing",
  "mode": 2,
  "mode_name": "player",
  "current_playlist": {
    "playlist": "Idle30",
    "type": "pause",
    "index": "1",
    "count": "1",
    "description": "Idle 30 seconds"
  },
  "current_sequence": "",
  "current_song": "",
  "seconds_elapsed": "5",
  "seconds_remaining": "25",
  "volume": 70,
  "scheduler": {
    "enabled": 1,
    "status": "playing",
    "currentPlaylist": {
      "playlistName": "Idle30",
      "scheduledStartTimeStr": "Sat Feb 26 @ 12:00 AM",
      "scheduledEndTimeStr": "Sun Feb 27 @ 12:00 AM",
      "secondsRemaining": 26517,
      "stopType": 0,
      "stopTypeStr": "Graceful"
    },
    "nextPlaylist": {
      "playlistName": "Idle30",
      "scheduledStartTimeStr": "Sun Feb 27 @ 12:00 AM - (Everyday)"
    }
  },
  "MQTT": { "configured": true, "connected": true },
  "volume": 70,
  "uuid": "M2-4c928e82-f67f-930d-56ee-7f883e62591b"
}
```

---

### GET `fppd/version`

Returns the current FPP version information.

**Response fields**

| Field | Type | Description |
|---|---|---|
| `version` | string | Full version string (e.g. `"4.x-master-904-ga13db6aa"`) |
| `majorVersion` | string | Major version number |
| `minorVersion` | string | Minor version number |
| `branch` | string | Git branch name |
| `fppdAPI` | string | API version (`"v1"`) |

**Example response**

```json
{
  "Status": "OK", "Message": "", "respCode": 200,
  "version": "4.x-master-904-ga13db6aa",
  "majorVersion": "4",
  "minorVersion": "1000",
  "branch": "master",
  "fppdAPI": "v1"
}
```

---

### GET `fppd/volume`

Returns the current output volume.

**Response**

```json
{ "Status": "OK", "Message": "", "respCode": 200, "volume": 70 }
```

---

### GET `fppd/effects`

Returns a list of currently running effects.

**Response**

```json
{
  "Status": "OK", "Message": "", "respCode": 200,
  "runningEffects": [
    { "id": 0, "name": "block_driveways" }
  ]
}
```

---

### GET `fppd/schedule`

Returns a projection of playlists and commands scheduled to run in the near future.

**Response fields**

| Field | Type | Description |
|---|---|---|
| `schedule.enabled` | int | 1 if the scheduler is enabled |
| `schedule.entries[]` | array | List of upcoming schedule entries |
| `entries[].id` | int | Schedule entry ID |
| `entries[].type` | string | `"playlist"` or `"command"` |
| `entries[].playlist` | string | Playlist name (for `type = "playlist"`) |
| `entries[].command` | string | Command name (for `type = "command"`) |
| `entries[].args` | array | Command arguments |
| `entries[].day` | int | Day of week (7 = every day) |
| `entries[].dayStr` | string | Human-readable day |
| `entries[].startTime` / `startDate` | string | Start time/date |
| `entries[].endTime` / `endDate` | string | End time/date |
| `entries[].repeat` | int | 1 if entry repeats |
| `entries[].enabled` | int | 1 if entry is enabled |
| `entries[].stopType` | int | 0 = Graceful |
| `entries[].multisyncCommand` | bool | Whether to multicast this command |

---

### GET `fppd/log`

Returns the current log level for each logging subsystem.

**Response**

```json
{
  "log": {
    "General": "warn", "ChannelOut": "warn", "ChannelData": "warn",
    "Command": "info", "Playlist": "debug", "Sequence": "warn",
    "Schedule": "warn", "HTTP": "warn", "MediaOut": "warn",
    "Control": "warn", "Sync": "warn", "E131Bridge": "warn",
    "Effect": "warn", "GPIO": "warn", "Settings": "warn", "Plugin": "warn"
  },
  "Status": "OK", "Message": "", "respCode": 200
}
```

---

### GET `fppd/e131stats`
### DELETE `fppd/e131stats`

**GET** — Returns current E1.31/DDP/ArtNet input statistics.

**DELETE** — Clears all E1.31/DDP/ArtNet input statistics.

**GET response**

```json
{
  "Status": "OK", "Message": "", "respCode": 200,
  "universes": [
    { "id": 1, "startChannel": 1, "bytesReceived": "0", "packetsReceived": "0", "errors": "0" }
  ]
}
```

---

### GET `fppd/multiSyncSystems`

Returns the list of known FPP instances discovered via MultiSync.

**Response fields (per system)**

| Field | Type | Description |
|---|---|---|
| `address` | string | IP address |
| `hostname` | string | Host name |
| `fppMode` | int | Mode integer |
| `fppModeString` | string | Mode name |
| `version` | string | FPP version string |
| `majorVersion` / `minorVersion` | int | Version numbers |
| `model` | string | Hardware model string |
| `type` | string | Hardware type |
| `typeId` | int | Hardware type ID |
| `channelRanges` | string | Channel ranges this instance handles |
| `lastSeen` | int | Unix timestamp of last contact |
| `lastSeenStr` | string | Formatted last-seen time |
| `local` | int | 1 if this is the local system |
| `multiSyncCapable` | int | 1 if MultiSync-capable |

---

### GET `fppd/multiSyncStats`

Returns packet statistics for MultiSync peers.

**Response fields (per system)**

| Field | Type | Description |
|---|---|---|
| `sourceIP` | string | IP address of the remote |
| `hostname` | string | Host name |
| `lastReceiveTime` | string | Timestamp of last received packet |
| `pktPing` / `pktBlank` / `pktCommand` / etc. | int | Packet counters by type |

---

### GET `fppd/testing`

Returns the current channel test mode configuration.

**Response**

```json
{
  "Status": "OK", "Message": "", "respCode": 200,
  "config": {
    "enabled": 1,
    "channelSet": "1-8392704",
    "channelSetType": "channelRange",
    "colorPattern": "FF000000FF000000FF",
    "cycleMS": 1000,
    "mode": "RGBChase",
    "subMode": "RGBChase-RGB"
  }
}
```

---

## playlist / playlists Endpoints

### GET `playlists`
### POST `playlists`

**GET** — Returns a list of all playlist names.

```json
["Playlist_1", "Playlist_2", "Playlist_3"]
```

**POST** — Creates a new playlist. Body is the full playlist object (same format as `GET playlist/:PlaylistName`).

**POST request body**

```json
{
  "name": "MyPlaylist",
  "mainPlaylist": [
    { "type": "pause", "enabled": 1, "playOnce": 0, "duration": 8 }
  ],
  "playlistInfo": { "total_duration": 8, "total_items": 1 }
}
```

**POST response**

```json
{ "Status": "OK", "Message": "" }
```

---

### GET `playlists/playable`

Returns a combined list of playlist names and FSEQ sequence filenames that can be started directly.

```json
["Playlist_1", "Playlist_2", "MySequence.fseq"]
```

---

### GET `playlists/validate`

Returns all playlists with validation status and any error messages.

```json
[
  { "name": "Test1", "valid": true, "message": [] },
  { "name": "Delete_Sequence", "valid": false, "message": ["Invalid Sequence Christmas Every Day_128.fseq"] }
]
```

---

### GET `playlists/stop`

Immediately stops the currently running playlist. No response body.

---

### GET `playlists/stopgracefully`

Gracefully stops the currently running playlist (waits for the current item to finish). No response body.

---

### GET `playlists/stopgracefullyafterloop`

Gracefully stops the currently running playlist after the current loop completes. No response body.

---

### GET `playlists/pause`

Pauses the currently running playlist. No response body.

---

### GET `playlists/resume`

Resumes a previously paused playlist. No response body.

---

### GET `playlist/:PlaylistName`
### POST `playlist/:PlaylistName`
### DELETE `playlist/:PlaylistName`

**GET** — Returns the named playlist in FPP JSON format.

Query parameter `?mergeSubs=1` recursively merges any sub-playlists into the response.

**GET response**

```json
{
  "name": "UploadTest",
  "mainPlaylist": [
    { "type": "pause", "enabled": 1, "playOnce": 0, "duration": 8 }
  ],
  "playlistInfo": { "total_duration": 8, "total_items": 1 }
}
```

**Playlist entry types** (the `type` field in `mainPlaylist`, `leadIn`, or `leadOut` arrays):

| Type | Description | Key fields |
|---|---|---|
| `sequence` | Sequence-only | `sequenceName` |
| `both` | Sequence + Media | `sequenceName`, `mediaName` |
| `media` | Media-only | `mediaName`, `fileMode` (`single`/`randomVideo`/`randomAudio`) |
| `pause` | Timed pause | `duration` (seconds) |
| `command` | FPP Command | `command`, `args[]`, `multisyncCommand` |
| `playlist` | Sub-playlist | `name` |
| `script` | Shell script | `scriptName`, `scriptArgs`, `blocking` |
| `branch` | Conditional branch | `branchTest`, `trueNext*`, `falseNext*` |
| `dynamic` | Dynamic content | `subType` (`file`/`plugin`/`url`) |
| `image` | Image overlay | `imagePath`, `modelName` |
| `url` | HTTP request | `method`, `url`, `data` |
| `remap` | Channel remap | `action`, `source`, `destination`, `count` |

Common fields on all entry types: `enabled` (int, 0/1), `playOnce` (int, 0/1), `note` (string, optional).

**POST** — Creates or replaces the named playlist. Body format is identical to the GET response.

**DELETE** — Deletes the named playlist.

---

### GET `playlist/:PlaylistName/start`

Starts the named playlist immediately.

---

### POST `playlist/:PlaylistName/:SectionName/item`

Appends an item to a section of the named playlist.

**Path parameters**

| Parameter | Description |
|---|---|
| `:PlaylistName` | Name of the playlist |
| `:SectionName` | Section name: `leadIn`, `mainPlaylist`, or `leadOut` |

**Request body** — A single playlist entry object (see entry types above).

```json
{ "type": "pause", "enabled": 1, "playOnce": 0, "duration": 8 }
```

**Response**

```json
{ "Status": "OK", "Message": "" }
```

---

## sequence Endpoints

### GET `sequence`

Returns a list of all FSEQ sequence file names (without the `.fseq` extension).

```json
["GreatestShow", "StPatricksDay", "Valentine"]
```

---

### GET `sequence/:SequenceName`
### POST `sequence/:SequenceName`
### DELETE `sequence/:SequenceName`

**GET** — Downloads the raw FSEQ binary file for the named sequence.

**POST** — Uploads a new FSEQ binary file. Body is the raw binary FSEQ data (`Content-Type: application/octet-stream`).

**DELETE** — Deletes the named FSEQ file.

---

### GET `sequence/:SequenceName/meta`

Returns metadata from the FSEQ file without downloading the full binary.

**Response**

```json
{
  "Name": "GreatestShow.fseq",
  "Version": "2.0",
  "ID": "1553194098754908",
  "StepTime": 25,
  "NumFrames": 10750,
  "MaxChannel": 84992,
  "ChannelCount": 84992
}
```

| Field | Type | Description |
|---|---|---|
| `Name` | string | File name |
| `Version` | string | FSEQ format version |
| `ID` | string | Unique sequence ID |
| `StepTime` | int | Milliseconds per frame |
| `NumFrames` | int | Total number of frames |
| `MaxChannel` | int | Highest channel number used |
| `ChannelCount` | int | Total number of channels |

---

### GET `sequence/:SequenceName/start/:startSecond`

Starts playing the named sequence from the specified time offset. Intended for testing purposes only — for normal playback use the `Start Playlist` command with the sequence name as the playlist name.

**Path parameters**

| Parameter | Description |
|---|---|
| `:SequenceName` | Sequence filename (with or without `.fseq` extension) |
| `:startSecond` | Start offset in seconds |

**Response**

```json
{ "status": "OK", "SequenceName": "single_line.fseq", "startSecond": "9" }
```

---

### GET `sequence/current/stop`

Stops the currently playing sequence (only valid if started via `sequence/:SequenceName/start/:startSecond`).

```json
{ "status": "OK" }
```

---

### GET `sequence/current/togglePause`

Pauses or resumes the current sequence (only valid if started via `sequence/:SequenceName/start/:startSecond`).

```json
{ "status": "OK" }
```

---

### GET `sequence/current/step`

Steps one frame forward when the sequence is paused.

```json
{ "status": "OK" }
```

---

### GET `sequence/current/stepBack`

Steps one frame backward when the sequence is paused.

```json
{ "status": "OK" }
```

---

## file / files Endpoints

These endpoints manage files in FPP's media directories. Valid directory names (`:DirName`) are:
`config`, `effects`, `events`, `images`, `logs`, `music`, `playlists`, `plugins`, `scripts`, `sequences`, `tmp`, `upload`, `videos`.

---

### GET `files/:DirName`

Lists files in the specified directory. Pass `?nameOnly=1` to return a plain array of filenames instead of full file objects.

**Response**

```json
{
  "status": "ok",
  "files": [
    {
      "name": "Christmas Every Day.mp3",
      "mtime": "09/23/20  07:47 PM",
      "sizeBytes": 7929000,
      "sizeHuman": "7.56MB",
      "playtimeSeconds": "03m:46s"
    }
  ]
}
```

| Field | Type | Description |
|---|---|---|
| `name` | string | Filename |
| `mtime` | string | Last-modified timestamp |
| `sizeBytes` | int | File size in bytes |
| `sizeHuman` | string | Human-readable file size |
| `playtimeSeconds` | string | Formatted playtime (audio/video files only) |

---

### GET `files/zip/:DirNames`

Downloads all files in one or more directories as a ZIP archive. `:DirNames` accepts a comma-separated list of directory names.

Response body is a raw ZIP file (`Content-Type: application/zip`).

---

### GET `file/:DirName(/:SubDir)/:Filename`

Downloads the specified file. An optional `:SubDir` subdirectory is supported.

**Query parameters**

| Parameter | Description |
|---|---|
| `?tail=N` | Return only the last N lines (useful for log files) |
| `?play=1` | Set Content-Type for in-browser playback rather than download |

Response body is the raw file contents.

---

### POST `file/:DirName(/:SubDir)/:Filename`

Uploads a file directly to the specified directory. Body is the raw file bytes.

Supports optional fragment parameters:

| Query parameter | Description |
|---|---|
| `?bs=N` | Block size for fragment upload |
| `?sb=N` | Starting block number for fragment upload |

> **Note:** Chunked Transfer-Encoding is not supported. Use the `PATCH` endpoint for large files.

**Response**

```json
{ "status": "OK", "file": "beepbeep.fseq", "dir": "sequences" }
```

To upload a music file: `POST api/file/music/MySong.mp3`  
To upload a video file: `POST api/file/videos/MyVideo.mp4`

---

### PATCH `file/:DirName`

Uploads a chunk of a file to the specified directory. Used for large files that must be sent in multiple requests. Files are staged in the `uploads` directory and then moved with `GET file/move/:Filename`.

**Required headers**

| Header | Description |
|---|---|
| `Upload-Name` | Target filename |
| `Upload-Length` | Total file size in bytes |
| `Upload-Offset` | Byte offset of this chunk |

**Response**

```json
{ "status": "OK", "file": "block_driveways.xbkp", "dir": "uploads" }
```

---

### GET `file/move/:Filename`

Moves the specified file from the `uploads` staging directory to the correct subdirectory. FPP determines the destination from the file extension (e.g. `.mp3` → `music`, `.mp4` → `videos`, `.fseq` → `sequences`).

**Response**

```json
{ "status": "OK" }
```

---

### POST `file/:DirName/copy/:source/:dest`

Copies a file within the specified directory from `:source` to `:dest`.

**Response**

```json
{ "status": "success", "original": "test.py", "new": "test2.py" }
```

---

### POST `file/:DirName/rename/:source/:dest`

Renames a file within the specified directory from `:source` to `:dest`.

**Response**

```json
{ "status": "success", "original": "test.py", "new": "test2.py" }
```

---

### DELETE `file/:DirName(/:SubDir)/:Filename`

Deletes the specified file. An optional `:SubDir` subdirectory is supported.

**Response**

```json
{ "status": "OK", "file": "block_driveways.xbkp", "dir": "uploads" }
```

---

## media Endpoints

### GET `media`

Returns a combined list of all music and video files available on the FPP instance.

```json
["Frosty.mp4", "Jingle_Bells.mp3"]
```

---

### GET `media/:MediaName/duration`

Returns the duration of the named media file in seconds.

**Path parameters**

| Parameter | Description |
|---|---|
| `:MediaName` | Media filename (e.g. `Frosty.mp4`) |

**Response**

```json
{
  "1min_720p29_2014-10-01.mp4": {
    "duration": 60.010666666667
  }
}
```

The top-level key is the requested filename and its value is an object with a single `duration` field (float, seconds).

---

### GET `media/:MediaName/meta`

Returns full FFprobe metadata for the named media file.

**Response fields**

| Field | Type | Description |
|---|---|---|
| `streams[]` | array | Audio/video stream descriptors |
| `streams[].index` | int | Stream index |
| `streams[].codec_name` | string | Short codec name (e.g. `"h264"`, `"aac"`) |
| `streams[].codec_long_name` | string | Full codec name |
| `streams[].codec_type` | string | `"video"` or `"audio"` |
| `streams[].width` / `height` | int | Video dimensions (video streams only) |
| `streams[].display_aspect_ratio` | string | Aspect ratio string (video streams only) |
| `streams[].pix_fmt` | string | Pixel format (video streams only) |
| `streams[].codec_time_base` | string | Codec time base fraction |
| `format.filename` | string | Full path on the FPP device |
| `format.format_name` | string | Container format identifiers |
| `format.format_long_name` | string | Human-readable container name |
| `format.duration` | string | Duration in seconds (as string) |
| `format.size` | string | File size in bytes (as string) |
| `format.bit_rate` | string | Bit rate in bps (as string) |
| `format.tags` | object | Embedded metadata tags (title, artist, encoder, etc.) |
| `programs` | array | Program list (typically empty) |
| `chapters` | array | Chapter list (typically empty) |

**Example response (abbreviated)**

```json
{
  "streams": [
    {
      "index": 0,
      "codec_name": "h264",
      "codec_type": "video",
      "width": 480,
      "height": 640,
      "display_aspect_ratio": "3:4",
      "pix_fmt": "yuv420p"
    },
    {
      "index": 1,
      "codec_name": "aac",
      "codec_type": "audio"
    }
  ],
  "format": {
    "filename": "/home/fpp/media/videos/Frosty.mp4",
    "format_name": "mov,mp4,m4a,3gp,3g2,mj2",
    "duration": "37.914000",
    "size": "12177878",
    "bit_rate": "2569579",
    "tags": {
      "title": "img_0872.mp4",
      "artist": "Avidemux"
    }
  }
}
```

---

## system Endpoints

### GET `system/info`

Returns basic static information about the FPP host.

**Response fields**

| Field | Type | Description |
|---|---|---|
| `HostName` | string | Configured hostname |
| `HostDescription` | string | User-configured description |
| `Platform` | string | Hardware platform (e.g. `"Raspberry Pi"`) |
| `Variant` | string | Hardware variant (e.g. `"Pi 4"`) |
| `Mode` | string | FPP operating mode (`"player"`) |
| `Version` | string | FPP version string |
| `Branch` | string | Git branch |
| `OSVersion` | string | OS image version |
| `OSRelease` | string | OS release string |
| `channelRanges` | string | Channel ranges this instance handles |
| `majorVersion` / `minorVersion` | int | Version numbers |
| `typeId` | int | Hardware type identifier |
| `uuid` | string | Unique device identifier |
| `Utilization.CPU` | float | CPU usage percentage |
| `Utilization.Memory` | float | Memory usage percentage |
| `Utilization.Uptime` | string | Uptime string |
| `Kernel` | string | Kernel version |
| `LocalGitVersion` | string | Local git hash |
| `RemoteGitVersion` | string | Remote git hash (for upgrade comparison) |
| `UpgradeSource` | string | Upgrade source (`"github.com"`) |
| `IPs` | string[] | List of IP addresses |

---

### GET `system/status`

Returns combined status including fppd, network, current playlist, scheduler, utilization, and MQTT state. Equivalent to `fppd/status` but with additional network interface details.

Optional query parameters: `&ip[]=<address>` — repeat for each additional FPP remote to query in the same call.

The response is a superset of `fppd/status` plus:

| Field | Type | Description |
|---|---|---|
| `interfaces[]` | array | Network interface details (name, flags, MTU, addresses) |
| `wifi[]` | array | Wireless interface details |
| `advancedView` | object | Additional version/hardware detail block |

---

### GET `system/volume`
### POST `system/volume`

**GET** — Returns the current volume. Works whether or not FPPD is running.

```json
{ "status": "OK", "method": "FPPD", "volume": 70 }
```

The `method` field is `"FPPD"` when the daemon is running, or `"Setting"` when reading from config.

**POST** — Sets the volume.

```json
{ "volume": 50 }
```

Response: `{ "status": "OK" }`

---

### GET `system/fppd/restart`

Restarts the FPPD daemon process.

Query parameter `?quick=1` reloads some configuration without a full restart.

```json
{ "status": "OK" }
```

---

### GET `system/fppd/start`

Starts the FPPD process if it is not already running.

```json
{ "status": "OK" }
```

---

### GET `system/fppd/stop`

Stops the FPPD process if it is running.

```json
{ "status": "OK" }
```

---

### GET `system/reboot`

Reboots the operating system.

```json
{ "status": "OK" }
```

---

### GET `system/shutdown`

Cleanly shuts down the Linux operating system.

```json
{ "status": "OK" }
```

---

## Common Patterns for a C# Client

### Base URL

```
http://<fpp-host>/api/
```

No authentication is required by default (FPP's `passwordEnable` setting controls optional basic auth).

### Response Envelope

Most endpoints wrap their payload in:

```json
{ "Status": "OK", "Message": "", "respCode": 200, ... }
```

Errors return non-200 `respCode` with a descriptive `Message`. A few simple endpoints return `{ "status": "OK" }` (lowercase) instead.

### Content-Type

- Send `Content-Type: application/json` on all POST/PUT requests.
- The FSEQ upload endpoint (`POST sequence/:SequenceName`) uses raw binary — send `Content-Type: application/octet-stream`.

### Recommended C# Models (sketch)

```csharp
// Shared envelope
public record FppResponse(string Status, string Message, int RespCode);

// fppd/status — key fields
public record FppStatus : FppResponse
{
    public string Fppd { get; init; }           // "running" / "stopped"
    public int StatusCode { get; init; }         // JSON: "status"
    public string StatusName { get; init; }      // JSON: "status_name"
    public FppCurrentPlaylist CurrentPlaylist { get; init; }
    public string CurrentSequence { get; init; }
    public string CurrentSong { get; init; }
    public int Volume { get; init; }
    public FppSchedulerStatus Scheduler { get; init; }
    public FppMqttStatus Mqtt { get; init; }
}

// system/info
public record FppSystemInfo
{
    public string HostName { get; init; }
    public string Platform { get; init; }
    public string Variant { get; init; }
    public string Version { get; init; }
    public string Mode { get; init; }
    public int MajorVersion { get; init; }
    public int MinorVersion { get; init; }
    public string Uuid { get; init; }
    public FppUtilization Utilization { get; init; }
    public string[] IPs { get; init; }
}

// playlist/:PlaylistName
public record FppPlaylist
{
    public string Name { get; init; }
    public FppPlaylistEntry[] LeadIn { get; init; }
    public FppPlaylistEntry[] MainPlaylist { get; init; }
    public FppPlaylistEntry[] LeadOut { get; init; }
    public FppPlaylistInfo PlaylistInfo { get; init; }
}

public record FppPlaylistEntry
{
    public string Type { get; init; }   // "sequence", "both", "media", "pause", "command", etc.
    public int Enabled { get; init; }
    public int PlayOnce { get; init; }
    // Type-specific fields vary — deserialize to JsonElement or use a discriminated union
}

// sequence/:SequenceName/meta
public record FppSequenceMeta
{
    public string Name { get; init; }
    public string Version { get; init; }
    public string Id { get; init; }
    public int StepTime { get; init; }
    public int NumFrames { get; init; }
    public int MaxChannel { get; init; }
    public int ChannelCount { get; init; }
}
```

Use `System.Text.Json` with `JsonSerializerOptions { PropertyNameCaseInsensitive = true }` to handle FPP's mixed-case field names.