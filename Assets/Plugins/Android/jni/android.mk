LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)
LOCAL_MODULE := secretKeyLib
LOCAL_SRC_FILES := secretKey.cpp
include $(BUILD_SHARED_LIBRARY)
