window.speech = {
    recognition: null,
    isRecording: false,

    startRecording: function (dotNetHelper) {
        if (!('webkitSpeechRecognition' in window) && !('SpeechRecognition' in window)) {
            dotNetHelper.invokeMethodAsync('OnError', 'Speech recognition not supported');
            return false;
        }

        const SpeechRecognition = window.SpeechRecognition || window.webkitSpeechRecognition;
        this.recognition = new SpeechRecognition();
        this.recognition.continuous = false;
        this.recognition.interimResults = true;
        this.recognition.lang = 'en-US';

        this.recognition.onresult = (event) => {
            let final = '';
            let interim = '';
            for (let i = event.resultIndex; i < event.results.length; i++) {
                if (event.results[i].isFinal) {
                    final += event.results[i][0].transcript;
                } else {
                    interim += event.results[i][0].transcript;
                }
            }
            dotNetHelper.invokeMethodAsync('OnInterim', interim);
            if (final) {
                dotNetHelper.invokeMethodAsync('OnFinal', final.trim());
            }
        };

        this.recognition.onerror = (e) => dotNetHelper.invokeMethodAsync('OnError', e.error);
        this.recognition.onend = () => dotNetHelper.invokeMethodAsync('OnEnd');

        this.recognition.start();
        this.isRecording = true;
        return true;
    },

    stopRecording: function () {
        if (this.recognition && this.isRecording) {
            this.recognition.stop();
            this.isRecording = false;
        }
    },

    speak: function (text, voiceName = "Microsoft Zira - English (United States)") {
        if (!('speechSynthesis' in window)) return;

        // Cancel any ongoing speech
        window.speechSynthesis.cancel();

        const utterance = new SpeechSynthesisUtterance(text);
        const voices = window.speechSynthesis.getVoices();

        // Try to find desired voice
        utterance.voice = voices.find(v => v.name.includes(voiceName)) || voices[0];
        utterance.rate = 0.9;
        utterance.pitch = 1.0;

        window.speechSynthesis.speak(utterance);
    },

    getVoices: function () {
        return (window.speechSynthesis.getVoices() || [])
            .filter(v => v.lang.startsWith('en'))
            .map(v => ({ name: v.name, lang: v.lang }))
            .slice(0, 10);
    }
}