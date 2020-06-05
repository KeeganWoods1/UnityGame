using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust;
    [SerializeField] float vertThrust;
    [SerializeField] float levelLoadDelay;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip explosion;
    [SerializeField] AudioClip outtroChime;
    [SerializeField] AudioClip introChime;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem levelFinishedParticles;
    [SerializeField] ParticleSystem deathParticles;

    [SerializeField] int nextScene;

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    private bool collisionsAreEnabled = true;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( state == State.Alive )
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !collisionsAreEnabled) { return; }

        switch (collision.gameObject.tag)
        {
            case "Ok":
                // do nothing
                break;

            case "Finish":
                FinishSequence();
                break;

            default:
                DyingSequence();
                break;
        }
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
           if (mainEngineParticles.isPlaying) mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {  
        rigidBody.AddRelativeForce(Vector3.up * vertThrust * Time.deltaTime); // Multiplied by deltaTime for framerate independence

        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        if (!mainEngineParticles.isPlaying) mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; 

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rcsThrust * Time.deltaTime);
        } 

        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rcsThrust * Time.deltaTime);
        }

        rigidBody.freezeRotation = false;

    }

    private void FinishSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(outtroChime);
        levelFinishedParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void DyingSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(explosion);
        deathParticles.Play();
        Invoke("ReloadCurrentScene", levelLoadDelay);
    }

    private void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        int nextSceneIndex = (currentSceneIndex + 1) % sceneCount;

        SceneManager.LoadScene(nextSceneIndex);
    }

    private void ReloadCurrentScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(currentSceneIndex);
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (collisionsAreEnabled)
            {
                print("invincible, you cannot be 'vinced");
                collisionsAreEnabled = false;
            }
            else
            {
                print("You are mortal again");
                collisionsAreEnabled = true;
            }
        }
    }
}
