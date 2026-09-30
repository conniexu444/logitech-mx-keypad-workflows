import AVFoundation
import Foundation

guard CommandLine.arguments.count == 4 else {
    fputs("usage: CodexVoiceRecorder OUTPUT.wav STOP_FILE STATUS_FILE\n", stderr)
    exit(2)
}

let outputURL = URL(fileURLWithPath: CommandLine.arguments[1])
let stopURL = URL(fileURLWithPath: CommandLine.arguments[2])
let statusURL = URL(fileURLWithPath: CommandLine.arguments[3])
func report(_ message: String) {
    try? message.write(to: statusURL, atomically: true, encoding: .utf8)
}
let permission = AVCaptureDevice.authorizationStatus(for: .audio)

if permission == .notDetermined {
    let semaphore = DispatchSemaphore(value: 0)
    AVCaptureDevice.requestAccess(for: .audio) { _ in semaphore.signal() }
    semaphore.wait()
}

guard AVCaptureDevice.authorizationStatus(for: .audio) == .authorized else {
    report("error:Microphone access was not granted. Enable Codex Keypad Voice Recorder in System Settings > Privacy & Security > Microphone.")
    fputs("Microphone access was not granted.\n", stderr)
    exit(3)
}

let settings: [String: Any] = [
    AVFormatIDKey: Int(kAudioFormatLinearPCM),
    AVSampleRateKey: 16_000.0,
    AVNumberOfChannelsKey: 1,
    AVLinearPCMBitDepthKey: 16,
    AVLinearPCMIsFloatKey: false,
    AVLinearPCMIsBigEndianKey: false,
]

do {
    let recorder = try AVAudioRecorder(url: outputURL, settings: settings)
    guard recorder.record() else {
        report("error:Could not start microphone recording.")
        fputs("Could not start microphone recording.\n", stderr)
        exit(4)
    }
    report("recording")

    let deadline = Date().addingTimeInterval(180)
    while !FileManager.default.fileExists(atPath: stopURL.path) && Date() < deadline {
        Thread.sleep(forTimeInterval: 0.1)
    }
    recorder.stop()

    guard FileManager.default.fileExists(atPath: stopURL.path) else {
        report("error:Recording timed out.")
        exit(7)
    }

    let attributes = try FileManager.default.attributesOfItem(atPath: outputURL.path)
    guard let fileSize = attributes[.size] as? Int, fileSize > 4_000 else {
        report("error:Recording was too short or empty.")
        fputs("Recording was too short or empty.\n", stderr)
        exit(5)
    }
    report("done")
} catch {
    report("error:Recording failed: \(error)")
    fputs("Recording failed: \(error)\n", stderr)
    exit(6)
}
