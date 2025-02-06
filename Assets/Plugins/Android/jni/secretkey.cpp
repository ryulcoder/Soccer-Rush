#include <jni.h>

extern "C"
JNIEXPORT jstring JNICALL
Java_com_mycompany_mygame_SecretKeyProvider_getSecretKey(JNIEnv* env, jobject obj) {
    // ����ȭ�� Ű (XOR ��ȣȭ�� ����)
    static char key[] = "\x4F\x72\x73\x34\x67\x70\x74";  // ���� Ű�� XOR ��ȣȭ�� ��

    // ��ȣȭ (XOR ����)
    for (int i = 0; i < sizeof(key) - 1; i++) {
        key[i] ^= 0xAA;  // XOR �������� ���� Ű ����
    }

    // ��ȣȭ�� Ű�� ���ڿ��� ��ȯ
    return env->NewStringUTF(key);
}
