using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 250.0f;
    [SerializeField] float mainThrust = 20.0f;
    [SerializeField] float levelLoadDelay = 2.0f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deadSound;
    [SerializeField] AudioClip nextLevelSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deadParticles;
    [SerializeField] ParticleSystem nextLevelParticles;

    bool collisionDisabled = false;


    enum State { Alive, Die, Trancending};
    State state=State.Alive;

    Rigidbody rigidBody;
    AudioSource audiosource;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audiosource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            Rotate();
            Thrust();
        }

        if(Debug.isDebugBuild)
        RespondToDebugKeys();
    }

    void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();

        }
        else
        {
            audiosource.Stop();
            if(mainEngineParticles.IsAlive())
            mainEngineParticles.Stop();
        }
           

    }

    void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust* Time.deltaTime);
        if (!audiosource.isPlaying)
            audiosource.PlayOneShot(mainEngine);
        mainEngineParticles.Play();
    }
    private void Rotate()
    {
        rigidBody.freezeRotation = true;
        float rotation = Time.deltaTime * rcsThrust;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward* rotation);
        }
        else
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward* rotation);
        }
        rigidBody.freezeRotation = false;
    }

    void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            collisionDisabled = !collisionDisabled;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionDisabled)
            return;
        print("Collision");
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                print(" with friendly");
                break;
            case "Finish":
                StartSuccessSequence();
                break;

            default: print("dead");
                StartDeathSequence();
                break;
               
        }
    }

    void StartSuccessSequence()
    {
        //print("Finish");
        audiosource.Stop();
        state = State.Trancending;
        nextLevelParticles.Play();
        audiosource.PlayOneShot(nextLevelSound);

        Invoke("LoadNextScene", levelLoadDelay);
       

    }

    void StartDeathSequence()
    {
        state = State.Die;
        audiosource.Stop();
        deadParticles.Play();
        audiosource.PlayOneShot(deadSound);
        Invoke("SceneAfterDead", levelLoadDelay);

    }

    void SceneAfterDead()
    {
        if (SceneManager.GetActiveScene().name.ToString() == "Level3")
            SceneManager.LoadScene(1);
        else
            SceneManager.LoadScene(0);
    }
    void LoadNextScene()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        int nextBuildIndex = (buildIndex+1)%SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextBuildIndex);
    }


}
