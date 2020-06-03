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

    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    [SerializeField] int nextScene;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

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
        rigidBody.AddRelativeForce(Vector3.up * vertThrust);

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
            transform.Rotate(Vector3.forward * rcsThrust);
        } 

        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rcsThrust);
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
        Invoke("LoadFirstScene", levelLoadDelay);
    }

    private void LoadFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
