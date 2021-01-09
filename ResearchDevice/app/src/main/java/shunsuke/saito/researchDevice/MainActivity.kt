package shunsuke.saito.researchDevice

import android.os.Bundle
import android.os.Debug
import android.support.wearable.activity.WearableActivity
import android.util.Log
import android.widget.Button
import android.widget.TextView

class MainActivity : WearableActivity() {
    var count: Int = 0

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        // Enables Always-on
        setAmbientEnabled()

        val countText: TextView = findViewById(R.id.countText)
        val countButton: Button = findViewById(R.id.countButton)

        countButton.setOnClickListener {
            count++
            countText.text = count.toString()
        }
    }
}