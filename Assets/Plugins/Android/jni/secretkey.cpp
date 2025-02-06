#include <jni.h>

extern "C"
JNIEXPORT jstring JNICALL
Java_com_mycompany_mygame_SecretKeyProvider_getSecretKey(JNIEnv* env, jobject obj) {
    // 난독화된 키 (XOR 암호화된 형태)
    static char key[] = "\x4F\x72\x73\x34\x67\x70\x74";  // 실제 키를 XOR 암호화한 값

    // 복호화 (XOR 연산)
    for (int i = 0; i < sizeof(key) - 1; i++) {
        key[i] ^= 0xAA;  // XOR 연산으로 원래 키 복원
    }

    // 복호화된 키를 문자열로 반환
    return env->NewStringUTF(key);
}
