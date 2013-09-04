/*
 * low-level API for FmStick
 * $Id$
 * (part of FmStick).
 *
 * (C) 2011 Serge <piratfm@gmail.com>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#ifndef FMTX_H
#define FMTX_H

#ifdef FMTX
#define EXTERN
#define ASSIGN (x) =x
#else
#define EXTERN extern
#define ASSIGN(x)
#endif

#ifdef _WIN32
#define FMTX_API_EXPORT __declspec(dllexport)
#ifndef _MSC_VER
#define FMTX_API_CALL  _stdcall
#endif
#define FMTX_API_CALL
#else
#define FMTX_API_EXPORT
#define FMTX_API_CALL
#endif

#include <inttypes.h>

#ifdef __cplusplus
extern "C" {
#endif

//MSB: Command Types
#define PCRequestError      0x80
#define PCTransfer          0x02
//MSB: Request Types
#define RequestDone         0x80

enum {
        RequestNone = 0,
        RequestCpuId,
        RequestSi4711Reset,     	//reset
        RequestSi4711Access,    	//low level access
        RequestSi4711GetProp,   	//medium level get prop
        RequestSi4711SetProp,   	//medium level set prop
        RequestSi4711PowerStatus,
        RequestSi4711PowerUp,   	//high level power up
        RequestSi4711PowerDown, 	//high level power down
        RequestSi4711AudioEnable,       //this MUST BE!!! called after setting config to enable audio.
        RequestSi4711AudioDisable,
        RequestEepromSectionRead,
        RequestEepromSectionWrite,
        RequestSi4711AsqStatus,
        RequestSi4711TuneStatus,
        RequestUnknown
};

enum {
        SI4711_OK = 0,
        SI4711_TIMEOUT,
        SI4711_COMM_ERR,
        SI4711_BAD_ARG,
        SI4711_NOCTS,
        SI4711_ERROR_UNKNOWN
};

enum {
    RTPLUS_DUMMY_CLASS = 0,

    RTPLUS_ITEM_TITLE,
    RTPLUS_ITEM_ALBUM,
    RTPLUS_ITEM_TRACKNUMBER,
    RTPLUS_ITEM_ARTIST,
    RTPLUS_ITEM_COMPOSITION,
    RTPLUS_ITEM_MOVEMENT,
    RTPLUS_ITEM_CONDUCTOR,
    RTPLUS_ITEM_COMPOSER,
    RTPLUS_ITEM_BAND,
    RTPLUS_ITEM_COMMENT,
    RTPLUS_ITEM_GENRE,

    RTPLUS_INFO_NEWS,
    RTPLUS_INFO_NEWS_LOCAL,
    RTPLUS_INFO_STOCKMARKET,
    RTPLUS_INFO_SPORT,
    RTPLUS_INFO_LOTTERY,
    RTPLUS_INFO_HOROSCOPE,
    RTPLUS_INFO_DAILY_DIVERSION,
    RTPLUS_INFO_HEALTH,
    RTPLUS_INFO_EVENT,
    RTPLUS_INFO_SZENE,
    RTPLUS_INFO_CINEMA,
    RTPLUS_INFO_STUPIDITY_MACHINE,
    RTPLUS_INFO_DATE_TIME,
    RTPLUS_INFO_WEATHER,
    RTPLUS_INFO_TRAFFIC,
    RTPLUS_INFO_ALARM,
    RTPLUS_INFO_ADVERTISEMENT,
    RTPLUS_INFO_URL,
    RTPLUS_INFO_OTHER,

    RTPLUS_STATIONNAME_SHORT,
    RTPLUS_STATIONNAME_LONG,
    /*  Category Code RT+ Class:
        0
        1 ITEM.TITLE
        2 ITEM.ALBUM
        3 ITEM.TRACKNUMBER
        4 ITEM.ARTIST
        5 ITEM.COMPOSITION
        6 ITEM.MOVEMENT
        7 ITEM.CONDUCTOR
        8 ITEM.COMPOSER
        9 ITEM.BAND
        10 ITEM.COMMENT
        11 ITEM.GENRE
        12 INFO.NEWS
        13 INFO.NEWS.LOCAL
        14 INFO.STOCKMARKET
        15 INFO.SPORT
        16 INFO.LOTTERY
        17 INFO.HOROSCOPE
        18 INFO.DAILY_DIVERSION
        19 INFO.HEALTH
        20 INFO.EVENT
        21 INFO.SZENE
        22 INFO.CINEMA
        23 INFO.STUPIDITY_MACHINE
        24 INFO.DATE_TIME
        25 INFO.WEATHER
        26 INFO.TRAFFIC
        27 INFO.ALARM
        28 INFO.ADVERTISEMENT
        29 INFO.URL
        30 INFO.OTHER
        31 STATIONNAME.SHORT
        32 STATIONNAME.LONG
*/
    RTPLUS_PROGRAMME_NOW,
    RTPLUS_PROGRAMME_NEXT,
    RTPLUS_PROGRAMME_PART,
    RTPLUS_PROGRAMME_HOST,
    RTPLUS_PROGRAMME_EDITORIAL_STAFF,
    RTPLUS_PROGRAMME_FREQUENCY,
    RTPLUS_PROGRAMME_HOMEPAGE,
    RTPLUS_PROGRAMME_SUBCHANNEL,

    RTPLUS_PHONE_HOTLINE,
    RTPLUS_PHONE_STUDIO,
    RTPLUS_PHONE_OTHER,

    RTPLUS_SMS_STUDIO,
    RTPLUS_SMS_OTHER,

    RTPLUS_EMAIL_HOTLINE,
    RTPLUS_EMAIL_STUDIO,
    RTPLUS_EMAIL_OTHER,

    RTPLUS_MMS_OTHER,

    RTPLUS_CHAT,
    RTPLUS_CHAT_CENTER,

    RTPLUS_VOTE_QUESTION,
    RTPLUS_VOTE_CENTER,
/*
        33 PROGRAMME.NOW
        34 PROGRAMME.NEXT
        35 PROGRAMME.PART
        36 PROGRAMME.HOST
        37 PROGRAMME.EDITORIAL_STAFF
        38 PROGRAMME.FREQUENCY
        39 PROGRAMME.HOMEPAGE
        40 PROGRAMME.SUBCHANNEL
        41 PHONE.HOTLINE
        42 PHONE.STUDIO
        43 PHONE.OTHER
        44 SMS.STUDIO
        45 SMS.OTHER
        46 EMAIL.HOTLINE
        47 EMAIL.STUDIO
        48 EMAIL.OTHER
        49 MMS.OTHER
        50 CHAT
        51 CHAT.CENTER
        52 VOTE.QUESTION
        53 VOTE.CENTER
*/
    RTPLUS_PLACE = 59,
    RTPLUS_APPOINTMENT,
    RTPLUS_IDENTIFIER,
    RTPLUS_PURCHASE,
    RTPLUS_GET_DATA
};


/** \brief FMTX mode enumeration. */
typedef enum {
    FMTX_MODE_NONE,            /*!< No FMTX detected. */
    FMTX_MODE_POWER_DOWN,      /*!< FMTX present in power-down mode. */
    FMTX_MODE_POWER_UP,        /*!< FMTX present in power-up mode. */
    FMTX_MODE_TRANSMITTING,    /*!< FMTX present in running mode. */
    FMTX_MODE_OK,              /*!< FMTX present in running mode and command is successfully. */
} FMTX_MODE_ENUM; // The current mode of the FMTX: none inserted, in bootloader mode or in normal application mode

typedef enum {
    FMTX_SPECIAL_FRONTEND,    /*!< Do Stuff on-the-go. */
    FMTX_SPECIAL_EEPROM       /*!< Don't do stuff in-real, just create EEPROM image. */
} FMTX_SPECIAL_ENUM;



/*********************************************************
 * EEPROM Info
 *********************************************************/
#define TX_CONFIG_OFFSET 	0x1E00
#define FMTX_EEPROM_CMDSIZE     0x08
#define FMTX_MAX_EEPROM_CMDS    62

typedef struct {
    uint16_t syncWord;
    uint8_t numCmds;
    uint8_t startupFlag;
    //unused 5 bytes;
    uint8_t commandData[FMTX_MAX_EEPROM_CMDS*FMTX_EEPROM_CMDSIZE];
} eeprom_data;



/*********************************************************
 * Shared Vars
 *********************************************************/
extern FMTX_MODE_ENUM fmtxCmdStatus;
extern FMTX_SPECIAL_ENUM fmtxCmdSpecial;

/*********************************************************
 * High-level API
 *********************************************************/
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxEEPROMInfoClean(void);
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxEEPROMReadConfig(void);
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxEEPROMWriteConfig(int autorun);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxEEPROMGetStartupFlag(void);
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxEEPROMSetStartupFlag(uint8_t flag, int force);
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxEEPROMWriteFwChunk(const uint8_t *data, int size, uint16_t offset);
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxEEPROMReadFwChunk(uint8_t *data, int size, uint16_t offset);

/*********************************************************
 * Low-level API
 *********************************************************/
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxIoAppDeviceFound(void);
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxIoAppFeReset(void);
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxIoAppFeGetMode(int *transmitting);
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxIoAppFeUp(void);
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxIoAppFeDown(void);
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxIoAppIdString(char *cpuid, char *rev);
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxIoAppAsqStatus(uint8_t *flags, int8_t *level);


EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxIoAppSetProperty(int16_t i16Prop, int16_t i16Val);
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxIoAppGetProperty(int16_t i16Prop, int16_t *pi16Val);

EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxIoAppCommand(uint8_t u8Cmd, uint8_t *pu8Resp, uint8_t u8Amount, ...);



/*********************************************************
 * Medium-level API
 *********************************************************/
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioGetAcompFlags();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioSetAcompFlags(uint8_t u8Val);
EXTERN FMTX_API_EXPORT FMTX_API_CALL int16_t fmtxAudioGetAcompThreshold();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioSetAcompThreshold(int16_t i16Val);
EXTERN FMTX_API_EXPORT FMTX_API_CALL double fmtxAudioGetAcompAttackTime();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioSetAcompAttackTime(double dVal);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioGetAcompReleaseTimeId();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioSetAcompReleaseTimeId(uint8_t u8Val);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioGetAcompGain();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioSetAcompGain(uint8_t u8Val);
EXTERN FMTX_API_EXPORT FMTX_API_CALL double fmtxAudioGetLimiterReleaseTime();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioSetLimiterReleaseTime(double dVal);

/* audio signal measurement */
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioGetAsqIntFlags();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudiosetAsqIntFlags(uint8_t u8Val);
EXTERN FMTX_API_EXPORT FMTX_API_CALL int8_t fmtxAudioGetAsqLevelLow();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioSetAsqLevelLow(int8_t i8Val);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint16_t fmtxAudioGetAsqDurationLow();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioSetAsqDurationLow(uint16_t u16Val);
EXTERN FMTX_API_EXPORT FMTX_API_CALL int8_t fmtxAudioGetAsqLevelHigh();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioSetAsqLevelHigh(int8_t i8Val);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint16_t fmtxAudioGetAsqDurationHigh();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioSetAsqDurationHigh(uint16_t u16Val);

/* digital audio interface */
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxIoAppEnableAudio(void);
EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxIoAppDisableAudio(void);
/* analogue audio interface */
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint16_t fmtxAudioGetInputLevels();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioSetInputLevels(uint16_t atten, uint16_t level);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint16_t fmtxAudioGetInputMute();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxAudioSetInputMute(uint16_t u16Val);

/* transmitting parameters */
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxTransmitterGetComponentFlags();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxTransmitterSetComponentFlags(uint8_t u8Val);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxTransmitterGetPreemphasisId();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxTransmitterSetPreemphasisId(uint8_t u8Val);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint32_t fmtxTransmitterGetAudioDeviation();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxTransmitterSetAudioDeviation(uint32_t u32Val);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint32_t fmtxTransmitterGetPilotDeviation();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxTransmitterSetPilotDeviation(uint32_t u32Val);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint32_t fmtxTransmitterGetRDSDeviation();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxTransmitterSetRDSDeviation(uint32_t u32Val);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint16_t fmtxTransmitterGetPilotFrequency();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxTransmitterSetPilotFrequency(uint16_t u16Val);

EXTERN FMTX_API_EXPORT FMTX_API_CALL FMTX_MODE_ENUM fmtxTransmitterGetTuneStatus(double *freq, uint8_t *power, double *rfcap);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxTransmitterSetTuneFreq(double freq);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxTransmitterSetTunePower(uint8_t power, double rfcap);


/* RDS parameters */
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint16_t fmtxRDSGetPI();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t  fmtxRDSSetPI(uint16_t u16Val);

EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSGetPsMixId();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSSetPsMixId(uint8_t u8Val);

EXTERN FMTX_API_EXPORT FMTX_API_CALL uint16_t fmtxRDSGetPsMiscFlags();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t  fmtxRDSSetPsMiscFlags(uint16_t u16Val);

EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSGetPsRepeatCount();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSSetPsRepeatCount(uint8_t u8Val);

EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSGetPsMessageCount();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSSetPsMessageCount(uint8_t u8Val);

EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSGetPsAFStatus(double *freq);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSSetPsAF(double freq, uint8_t enabled);

EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSGetFifoSize();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSSetFifoSize(uint8_t u8Val);

/* hackish commands for getting PS and RT data from eeprom */
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSGetPsMessageById(uint8_t Id, char *messages);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSSetPsMessageById(uint8_t Id, char *messages);

EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSGetRtMessage(char *messages); //returns one long string with RT message
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSSetRtMessage(const char *message);
/* RTPlus info must be setted after Rt message due fmtxRDSSetRtMessage() flusher circular buffer */
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSSetRtPlusInfo(int content1, int content1_pos, int content1_len,
                                                                   int content2, int content2_pos, int content2_len);
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSSendTimeStamp();
EXTERN FMTX_API_EXPORT FMTX_API_CALL uint8_t fmtxRDSSendCustomGroup(uint8_t flags, uint8_t B0, uint8_t B1, uint8_t C0, uint8_t C1, uint8_t D0, uint8_t D1);




#ifdef __cplusplus
}
#endif

#endif // FMTX_H
