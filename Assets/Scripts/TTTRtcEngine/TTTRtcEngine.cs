using UnityEngine;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace TTTRtcEngine
{
	public enum ERROR_CODE
	{
		INVALID_CHANNEL_NAME = 9000,
		ENTER_TIMEOUT        = 9001,
		ENTER_FAILED         = 9002,
		ENTER_VERIFYFAILED   = 9003,
		ENTER_BADVERSION     = 9004,
		ENTER_UNKNOWN        = 9005,
	};

	public enum LOG_FILTER
	{
		OFF      = 0,
		DEBUG    = 0x080f,
		INFO     = 0x000f,
		WARNING  = 0x000e,
		ERROR    = 0x000c,
	};

	public enum CHANNEL_PROFILE
	{
		GAME_FREE_MODE = 2,
	};

	public enum VIDEO_PROFILE
	{
		             //   res     fps   kbps
		_120P  =  0, // 160x120    15     65
		_180P  = 10, // 320x180    15    140
		_240P  = 20, // 320x240    15    200
		_360P  = 30, // 640x360    15    400
		_480P  = 40, // 640x480    15    500
		_720P  = 50, // 1280x720   15   1130
		_1080P = 60, // 1920x1080  15   2080
	};

	public enum USER_OFFLINE_REASON
	{
		QUIT            = 0,
		DROPPED         = 1,
		BECOME_AUDIENCE = 2,
	};

	public enum AUDIO_ROUTE
	{
		HEADSET   = 0, //耳机或蓝牙
		SPEAKERE  = 1, //扬声器
		HEADPHONE = 2, //手机听筒
	};

	public struct RtcStats
	{
		public uint duration;
		public uint txBytes;
		public uint rxBytes;
		public ushort txKBitRate;
		public ushort rxKBitRate;
		public ushort txAudioKBitRate;
		public ushort rxAudioKBitRate;
		public uint users;
	};

	public struct AudioVolumeInfo
	{
		public uint uid;
		public uint volume; // [0, 255]
	};


	public class IRtcEngine
	{
	#region DllImport
		#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		public const string MyLibName = "TTTRtcEngine";
		#else
		#if UNITY_IPHONE
		public const string MyLibName = "__Internal";
		#else
		public const string MyLibName = "TTTRtcEngine";
		#endif
		#endif
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int getMessageCount();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern IntPtr getMessage(); // caller free the returned char * (through freeObject)
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern void freeObject(IntPtr obj);

		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern void createEngine(string appId);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern void deleteEngine();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern IntPtr getSdkVersion();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int joinChannel(string channelKey, string channelName, uint uid);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int leaveChannel();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int enableVideo();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int disableVideo();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int enableLocalVideo(int enabled);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int startPreview();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int stopPreview();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int setEnableSpeakerphone(int enabled);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int isSpeakerphoneEnabled();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int setDefaultAudioRoutetoSpeakerphone(int enabled);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int enableAudioVolumeIndication(int interval, int smooth);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int startAudioMixing(string filePath, int loopBack, int replace, int cycle);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int stopAudioMixing();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int pauseAudioMixing();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int resumeAudioMixing();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int adjustAudioMixingVolume(int volume);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int getAudioMixingDuration();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int getAudioMixingCurrentPosition();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern  int setAudioMixingPosition(int pos);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int muteLocalAudioStream(int mute);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int muteAllRemoteAudioStreams(int mute);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int muteRemoteAudioStream(int uid, int mute);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int switchCamera();
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern  int setVideoProfile(int profile, int swapWidthAndHeight);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern  int muteAllRemoteVideoStreams(int mute);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern  int muteRemoteVideoStream(int uid, int mute);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern  int setChannelProfile(int profile);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern  int setHighQualityAudioParametersWithFullband(int fullband, int stereo, int fullBitrate);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern  int setLogFilter(uint filter);
		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern  int setLogFile(string filePath);

		[DllImport (MyLibName, CharSet = CharSet.Ansi)]
		private static extern int updateTexture(int tex, uint uid, int width, int height);
	#endregion // DllImport

	#region Engine callbacks
		//------------------------------------------------------------------------------------------
		// 定义回调
		//------------------------------------------------------------------------------------------
		public delegate void JoinChannelSuccessHandler (string channelName, uint uid, int elapsed);
		public JoinChannelSuccessHandler OnJoinChannelSuccess;

		public delegate void ConnectionLostHandler ();
		public ConnectionLostHandler OnConnectionLost;

		public delegate void UserJoinedHandler (uint uid, int elapsed);
		public UserJoinedHandler OnUserJoined;

		public delegate void UserOfflineHandler (uint uid, USER_OFFLINE_REASON reason);
		public UserOfflineHandler OnUserOffline = null;

		public delegate void LeaveChannelHandler (RtcStats stats);
		public LeaveChannelHandler OnLeaveChannel;

		public delegate void ReportAuidoLevelHandler (uint uid, int audioLevel);
		public ReportAuidoLevelHandler OnReportAuidoLevel;

		public delegate void UserMutedHandler (uint uid, bool muted);
		public UserMutedHandler OnUserMuted;

		public delegate void SDKErrorHandler (ERROR_CODE error, string msg);
		public SDKErrorHandler OnError;

		public delegate void RtcStatsHandler (RtcStats stats);
		public RtcStatsHandler OnRtcStats;

		public delegate void AudioMixingFinishedHandler ();
		public AudioMixingFinishedHandler OnAudioMixingFinished;

		public delegate void AudioRouteChangedHandler (AUDIO_ROUTE route);
		public AudioRouteChangedHandler OnAudioRouteChanged;

		public delegate void onFirstLocalVideoFrameHandler (uint uid, int width, int height, int elapsed);
		public onFirstLocalVideoFrameHandler OnFirstLocalVideoFrame;

		public delegate void onFirstRemoteVideoFrameDecodedHandler (uint uid, int width, int height, int elapsed);
		public onFirstRemoteVideoFrameDecodedHandler OnFirstRemoteVideoFrameDecoded;
	#endregion

		private IRtcEngine(string appId) {
			createEngine (appId);
		}

		/**
		 * 查询 SDK 版本号
		 *
		 * @return SDK 版本号的字符串
		 */
		public static string GetSdkVersion () {
			return Marshal.PtrToStringAnsi (getSdkVersion ());
		}

		/**
		 * 获取错误描述
		 * @param [in] code 错误代码
		 * @return 错误代码对应的错误描述
		 */
		public static string GetErrorDescription (ERROR_CODE code) {
			string error = string.Empty;
			switch (code) {
			case ERROR_CODE.INVALID_CHANNEL_NAME:
				error = "无效的房间名称";
				break;
			case ERROR_CODE.ENTER_TIMEOUT:
				error = "超时,10秒未收到服务器返回结果";
				break;
			case ERROR_CODE.ENTER_FAILED:
				error = "无法连接服务器";
				break;
			case ERROR_CODE.ENTER_VERIFYFAILED:
				error = "验证码错误";
				break;
			case ERROR_CODE.ENTER_BADVERSION:
				error = "版本错误";
				break;
			case ERROR_CODE.ENTER_UNKNOWN:
				error = "未知错误";
				break;
			};
			return error;
		}

		/**
		 * 设置频道属性
		 *
		 * @param profile 频道模式
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int SetChannelProfile(CHANNEL_PROFILE profile) {
			return setChannelProfile ((int)profile);
		}

		/**
		* 设置日志文件过滤器
		*
		* @param filter 日志过滤器（1: INFO, 2: WARNING, 4: ERROR, 8: FATAL, 0x800: DEBUG）
		* @return 0: 方法调用成功，<0: 方法调用失败。
		*/
		public int SetLogFilter (LOG_FILTER filter) {
			return setLogFilter ((uint)filter);
		}

		/**
		* 设置SDK输出的日志文件 应用程序必须保证指定的目录存在而且可写
		*
		* @param filePath 日志文件的完整路径。该日志文件为UTF-8编码。
		* @return 0: 方法调用成功，<0: 方法调用失败。
		*/
		public int SetLogFile (string filePath) {
			return setLogFile (filePath);
		}

		/**
		 * 加入频道，在同一个频道内的用户可以互相通话。
		 *
		 * @param [in] channelName 标识通话的频道名称
		 * @param [in] uid  用户 ID
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int JoinChannel (string channelName, uint uid) {
			return JoinChannelByKey(null, channelName, uid);
		}

		public int JoinChannelByKey (string channelKey, string channelName, uint uid) {
			return joinChannel(channelKey, channelName, uid);
		}

		/**
		 * 离开频道
		 *
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int LeaveChannel () {
			return leaveChannel ();
		}

		// 获取消息队列里的消息数量
		public int GetMessageCount () {
			return getMessageCount ();
		}

		/**
		 * 允许/禁止往网络发送本地音频流
		 *
		 * @param mute true: 麦克风静音，false: 取消静音。
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int MuteLocalAudioStream (bool mute) {
			return muteLocalAudioStream (mute ? 1 : 0);
		}

		/**
		 * 允许/禁止播放远端用户的音频流，即对所有远端用户进行静音与否。
		 *
		 * @param mute true: 停止播放所有收到的音频流，false: 允许播放所有收到的音频流。
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int MuteAllRemoteAudioStreams (bool mute) {
			return muteAllRemoteAudioStreams (mute ? 1 : 0);
		}

		/**
		 * 允许/禁止播放远端用户的音频流
		 *
		 * @param uid 用户 ID
		 * @param mute true: 停止播放指定用户的音频流，false: 允许播放指定用户的音频流。
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int MuteRemoteAudioStream (uint uid, bool mute) {
			return muteRemoteAudioStream ((int)uid, (mute ? 1 : 0));
		}

		/**
		 * 打开外放(扬声器)
		 * 
		 * @param speakerphone true: 切换到从外放(扬声器)出声，false: 语音会根据默认路由出声。
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int SetEnableSpeakerphone (bool speakerphone) {
			return setEnableSpeakerphone (speakerphone ? 1 : 0);
		}

		/**
		 * 修改默认的语音路由
		 * 
		 * @param speakerphone true: 默认路由改为外放(扬声器)，false: 默认路由改为听筒。
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int SetDefaultAudioRouteToSpeakerphone(bool speakerphone) {
			return setDefaultAudioRoutetoSpeakerphone (speakerphone ? 1 : 0);
		}

		/**
		 * 检查扬声器是否已开启
		 *
		 * @return true: 扬声器已开启，false: 扬声器未开启。
		 */
        public bool IsSpeakerphoneEnabled() {
            return isSpeakerphoneEnabled() != 0;
        }

		/**
		 * 切换前置/后置摄像头
		 *
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
        public int SwitchCamera() {
            return switchCamera();
        }

		/**
		 * 设置视频属性
		 * 
		 * @param profile            每个Profile对应一套视频参数，如分辨率、帧率、码率等。
         * @param swapWidthAndHeight 是否交换宽和高
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int SetVideoProfile(VIDEO_PROFILE profile, bool swapWidthAndHeight) {
			return setVideoProfile((int)profile, (swapWidthAndHeight ? 1 : 0));
        }

		/**
		 * 暂停/恢复发送本地视频流
		 *
		 * @param mute true: 停止向网络发送本地视频流，false: 允许向网络发送本地视频流。
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
        //public int MuteLocalVideoStream(bool mute) {
        //    return muteLocalVideoStream(mute ? 1 : 0);
        //}

		/**
		 * 允许/禁止播放所有人的视频流
		 *
		 * @param mute true: 停止播放所有收到的视频流，false: 允许播放所有收到的视频流。
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
        public int MuteAllRemoteVideoStreams(bool mute) {
            return muteAllRemoteVideoStreams(mute ? 1 : 0);
        }

		/**
		 * 暂停/恢复指定用户视频流。该方法不影响视频数据流的接收，只是不播放视频流。
		 *
		 * @param uid 指定远端用户的 UID
		 * @param mute true: 停止播放指定用户的视频流，false: 允许播放指定用户的视频流。
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
        public int MuteRemoteVideoStream(uint uid, bool mute) {
			return muteRemoteVideoStream((int)uid, (mute ? 1 : 0));
        }

        public int SetHighQualityAudioParametersWithFullband(bool fullband, bool stereo, bool fullBitrate){
            return setHighQualityAudioParametersWithFullband((fullband ? 1 : 0), (stereo ? 1 : 0), (fullBitrate ? 1 : 0));
        }

        public int SetAudioMixingPosition(int pos) {
			return setAudioMixingPosition(pos);
        }

		/**
		 * 启用/禁用说话者音量提示
		 *
		 * @param interval 指定音量提示的时间间隔（<=0: 禁用音量提示功能；>0: 提示间隔，单位为毫秒。建议设置到大于200毫秒。）
		 * @param smooth   平滑系数。默认可以设置为3。
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int EnableAudioVolumeIndication (int interval, int smooth) {
			return enableAudioVolumeIndication (interval, smooth);
		}

		/**
		 * 开始客户端本地混音
		 *
		 * @param filePath 指定需要混音的本地音频文件名和文件路径
		 * @param loopback true: 只有本地可以听到混音或替换后的音频流，false: 本地和对方都可以听到混音或替换后的音频流。
		 * @param replace  true: 音频文件内容将会替换本地录音的音频流，false: 音频文件内容将会和麦克风采集的音频流进行混音。
		 * @param cycle    指定音频文件循环播放的次数
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int StartAudioMixing (string filePath, bool loopback, bool replace, int cycle) {
			return startAudioMixing (filePath, (loopback ? 1 : 0), (replace ? 1 : 0), cycle);
		}

		/**
		 * 停止客户端本地混音
		 *
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int StopAudioMixing() {
			return stopAudioMixing ();
		}

		/**
		 * 暂停播放伴奏
		 *
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int PauseAudioMixing() {
			return pauseAudioMixing ();
		}

		/**
		 * 恢复播放伴奏
		 *
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int ResumeAudioMixing() {
			return resumeAudioMixing ();
		}

		/**
		 * adjust mixing audio file volume
		 *
		 * @param [in] volume range from 0 to 100
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int AdjustAudioMixingVolume (int volume) {
			return adjustAudioMixingVolume (volume);
		}

		/**
		 * get the duration of the specified mixing audio file
		 *
		 * @return return duration(ms)
		 */
		public int GetAudioMixingDuration() {
			return getAudioMixingDuration ();
		}

		/**
		 * get the current playing position of the specified mixing audio file
		 *
		 * @return return the current playing(ms)
		 */
		public int GetAudioMixingCurrentPosition() {
			return getAudioMixingCurrentPosition ();
		}

		/**
		 * 打开视频模式
		 *
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int EnableVideo () {
			return enableVideo ();
		}

		/**
		 * 关闭视频模式
		 *
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int DisableVideo () {
			return disableVideo ();
		}

		/**
		 * 禁用/启用本地视频功能
		 * 
		 * @param enabled true: 启用本地视频（默认），false: 禁用本地视频。
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int EnableLocalVideo (bool enabled) {
			return enableLocalVideo (enabled ? 1 : 0);
		}

		/**
		 * 启动本地视频预览
		 *
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public int StartPreview () {
			return startPreview ();
		}

		/**
		 * 停止本地视频预览
		 *
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */

		public int StopPreview () {
			return stopPreview ();
		}

		/**
		 * 更新纹理
		 *
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */

		// load data to texture
		// return value:
		public int UpdateTexture (int tex, uint uid, int width, int height) {
			return updateTexture (tex, uid, width, height);
		}

		// 获取/创建引擎实例
		public static IRtcEngine GetEngine (string appId)
		{
			if (instance == null) {
				instance = new IRtcEngine (appId);
			}
			return instance;
		}

		/**
		 * 销毁引擎实例
		 *
		 * @return 0: 方法调用成功，<0: 方法调用失败。
		 */
		public static void Destroy()
		{
			if (instance != null) {
				deleteEngine ();
				instance = null;
			}
		}

		// 查询引擎实例
		public static IRtcEngine QueryEngine()
		{
			return instance;
		}

		private static IRtcEngine instance = null;

		// 触发 SDK 事件
		public string Poll ()
		{
			IntPtr msg = getMessage ();
			string str = Marshal.PtrToStringAnsi(msg);
			freeObject (msg);

			if (string.IsNullOrEmpty(str))
				return string.Empty;
			Debug.Log ("TTT >>> " + str + " <<<");
			string[] sArray = str.Split ('\t');

			if (sArray [0].CompareTo ("onError") == 0) {
				if (OnError != null) {
					ERROR_CODE errCode = (ERROR_CODE)int.Parse (sArray [1]);
					OnError (errCode, GetErrorDescription(errCode));
				}
			} else if (sArray [0].CompareTo ("onConnectionLost") == 0) {
				if (OnConnectionLost != null) {
					OnConnectionLost ();
				}
			} else if (sArray [0].CompareTo ("onJoinChannelSuccess") == 0) {
				if (OnJoinChannelSuccess != null) {
					uint uid = (uint)int.Parse (sArray [2]);
					int elapsed = int.Parse (sArray [3]);
					OnJoinChannelSuccess (sArray [1], uid, elapsed);
				}
			} else if (sArray [0].CompareTo ("onLeaveChannel") == 0) {
				if (OnLeaveChannel != null) {
					int duration = int.Parse (sArray [1]);
					int txBytes = int.Parse (sArray [2]);
					int rxBytes = int.Parse (sArray [3]);
					int txAudioKBitrate = int.Parse (sArray [4]);
					int rxAudioKBitrate = int.Parse (sArray [5]);
					int txVideoKBitrate = int.Parse (sArray [6]);
					int rxVideoKBitrate = int.Parse (sArray [7]);
					int users = int.Parse (sArray [8]);

					RtcStats stats;
					stats.duration = (uint)duration;
					stats.txBytes = (uint)txBytes;
					stats.rxBytes = (uint)rxBytes;
					stats.txKBitRate = (ushort)(txAudioKBitrate + txVideoKBitrate);
					stats.rxKBitRate = (ushort)(rxAudioKBitrate + rxVideoKBitrate);
					stats.txAudioKBitRate = (ushort)txAudioKBitrate;
					stats.rxAudioKBitRate = (ushort)rxAudioKBitrate;
					stats.users = (uint)users;
					OnLeaveChannel (stats);
				}
			} else if (sArray [0].CompareTo ("onUserJoined") == 0) {
				if (OnUserJoined != null) {
					uint uid = (uint)int.Parse (sArray [1]);
					int elapsed = int.Parse (sArray [2]);
					OnUserJoined (uid, elapsed);
				}
			} else if (sArray [0].CompareTo ("onUserOffline") == 0) {
				if (OnUserOffline != null) {
					uint uid = (uint)int.Parse (sArray [1]);
					int reason = int.Parse (sArray [2]);
					OnUserOffline (uid, (USER_OFFLINE_REASON)reason);
				}
			} else if (sArray [0].Equals("onFirstLocalVideoFrame")) {
				if (OnFirstLocalVideoFrame != null) {
					int uid = int.Parse (sArray [1]);
					int width = int.Parse (sArray [2]);
					int height = int.Parse (sArray [3]);
					int elapsed = int.Parse (sArray [4]);
					OnFirstLocalVideoFrame ((uint)uid, width, height, elapsed);
				}
			} else if (sArray [0].CompareTo ("onFirstRemoteVideoFrameDecoded") == 0) {
				if (OnFirstRemoteVideoFrameDecoded != null) {
					int uid = int.Parse (sArray [1]);
					int width = int.Parse (sArray [2]);
					int height = int.Parse (sArray [3]);
					int elapsed = int.Parse (sArray [4]);
					OnFirstRemoteVideoFrameDecoded ((uint)uid, width, height, elapsed);
				}
			} else if (sArray [0].CompareTo ("onAudioRouteChanged") == 0) {
				if (OnAudioRouteChanged != null) {
					int routing = int.Parse (sArray [1]);
					OnAudioRouteChanged ((AUDIO_ROUTE)routing);
				}
			} else if (sArray [0].CompareTo ("onReportAuidoLevel") == 0) {
				if (OnReportAuidoLevel != null) {
					uint uid = (uint)int.Parse (sArray [1]);
					int auidoLevel = int.Parse (sArray [2]);
					OnReportAuidoLevel (uid, auidoLevel);
				}
			} else if (sArray [0].CompareTo ("onAudioMuted") == 0) {
				if (OnUserMuted != null) {
					uint uid = (uint)int.Parse (sArray [1]);
					bool muted = int.Parse (sArray [2]) != 0;
					OnUserMuted (uid, muted);
				}
			} else if (sArray [0].CompareTo ("onVideoEnabled") == 0) {
	
			} else if (sArray [0].CompareTo ("onLocalVideoStats") == 0) {

			} else if (sArray [0].CompareTo ("onRemoteVideoStats") == 0) {

			} else if (sArray [0].CompareTo ("onReportRtcStats") == 0) {
				if (OnRtcStats != null) {
					int j = 1;
					int duration = int.Parse(sArray[j++]);
					int txBytes = int.Parse(sArray[j++]);
					int rxBytes = int.Parse(sArray[j++]);
					int txAudioKBitrate = int.Parse(sArray[j++]);
					int rxAudioKBitrate = int.Parse(sArray[j++]);
					int txVideoKBitrate = int.Parse(sArray[j++]);
					int rxVideoKBitrate = int.Parse(sArray[j++]);
					int users = int.Parse(sArray[j++]);

					RtcStats stats;
					stats.duration = (uint)duration;
					stats.txBytes = (uint)txBytes;
					stats.rxBytes = (uint)rxBytes;
					stats.txKBitRate = (ushort)(txAudioKBitrate + txVideoKBitrate);
					stats.rxKBitRate = (ushort)(rxAudioKBitrate + rxVideoKBitrate);
					stats.txAudioKBitRate = (ushort)txAudioKBitrate;
					stats.rxAudioKBitRate = (ushort)rxAudioKBitrate;
					stats.users = (uint)users;
					OnRtcStats (stats);
				}
			} else {
			}

			return sArray [0];
		}
	}
}
