using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKYNET.Pages
{
    public class Index
    {
        public static string html(string styles, string javascript)
        {
            string result = @"

<!DOCTYPE html>
<html lang='en'>

<head>
    <meta charset='UTF-8'>
    <meta http-equiv='X-UA-Compatible' content='IE=edge'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link rel='stylesheet' href='c:/css/style.css'>
    <link rel='stylesheet' href='c:/css/helpers.css'>
</head>

<body class='m-0'>
    <!-- Header with options -->
    <header>
        <div class='filters'></div>
        <div class='logo'>
            <img src='c:/logo.png' alt='Dota 2 Logo'>
        </div>
        <div class='play-modes'></div>
    </header>

    <!-- Content Section -->
    <section class='content pt-8 px-24'>
        <!-- Action's Bar -->
        <div class='actions-bar'>
            <!-- Play Button -->
            <button class='action-btn play-game cursor-pointer mr-24'>
                <svg
                    class='mr-8'
                    xmlns='http://www.w3.org/2000/svg'
                    height='28px'
                    viewBox='0 0 24 24'
                    width='28px'
                >
                    <path d='M0 0h24v24H0z' fill='none'/>
                    <path d='M8 5v14l11-7z'/>
                </svg>
                <span>PLAY</span>
                <div class='play-game-fade'></div>
            </button>

            <!-- Stop Button -->
            <!-- <button class='action-btn stop-game cursor-pointer mr-24'>
                <svg
                    class='mr-8'
                    xmlns='http://www.w3.org/2000/svg'
                    height='24px'
                    viewBox='0 0 24 24'
                    width='24px'
                    fill='#000000'
                >
                    <path d='M0 0h24v24H0V0z' fill='none'/>
                    <path stroke='white' stroke-width='2' d='M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12 19 6.41z'/>
                </svg>
                <span>STOP</span>
                <div class='stop-game-fade'></div>
            </button> -->

            <div class='last-played mr-24'>
                <span class='title fg-light-gray'>LAST PLAYED</span>
                <span class='date fg-mid-gray'>Jun 7</span>
            </div>

            <div class='play-time mr-24 fg-light-gray'>
                <svg
                    class='mr-8'
                    xmlns='http://www.w3.org/2000/svg'
                    enable-background='new 0 0 24 24'
                    height='24px' viewBox='0 0 24 24'
                    width='24px'
                    fill='#000000'
                >
                    <g>
                        <rect fill='none' height='24' width='24'/>
                        <path d='M15.1,19.37l1,1.74c-0.96,0.44-2.01,0.73-3.1,0.84v-2.02C13.74,19.84,14.44,19.65,15.1,19.37z M4.07,13H2.05 c0.11,1.1,0.4,2.14,0.84,3.1l1.74-1C4.35,14.44,4.16,13.74,4.07,13z M15.1,4.63l1-1.74C15.14,2.45,14.1,2.16,13,2.05v2.02 C13.74,4.16,14.44,4.35,15.1,4.63z M19.93,11h2.02c-0.11-1.1-0.4-2.14-0.84-3.1l-1.74,1C19.65,9.56,19.84,10.26,19.93,11z M8.9,19.37l-1,1.74c0.96,0.44,2.01,0.73,3.1,0.84v-2.02C10.26,19.84,9.56,19.65,8.9,19.37z M11,4.07V2.05 c-1.1,0.11-2.14,0.4-3.1,0.84l1,1.74C9.56,4.35,10.26,4.16,11,4.07z M18.36,7.17l1.74-1.01c-0.63-0.87-1.4-1.64-2.27-2.27 l-1.01,1.74C17.41,6.08,17.92,6.59,18.36,7.17z M4.63,8.9l-1.74-1C2.45,8.86,2.16,9.9,2.05,11h2.02C4.16,10.26,4.35,9.56,4.63,8.9z M19.93,13c-0.09,0.74-0.28,1.44-0.56,2.1l1.74,1c0.44-0.96,0.73-2.01,0.84-3.1H19.93z M16.83,18.36l1.01,1.74 c0.87-0.63,1.64-1.4,2.27-2.27l-1.74-1.01C17.92,17.41,17.41,17.92,16.83,18.36z M7.17,5.64L6.17,3.89 C5.29,4.53,4.53,5.29,3.9,6.17l1.74,1.01C6.08,6.59,6.59,6.08,7.17,5.64z M5.64,16.83L3.9,17.83c0.63,0.87,1.4,1.64,2.27,2.27 l1.01-1.74C6.59,17.92,6.08,17.41,5.64,16.83z M13,7h-2v5.41l4.29,4.29l1.41-1.41L13,11.59V7z'/>
                    </g>
                </svg>
                <div class='container'>
                    <span class='title fg-light-gray'>PLAY TIME</span>
                    <span class='time fg-mid-gray'>24.0 horas</span>
                </div>
            </div>

            <div class='toolbox'>
                <button>
                    <svg
                        xmlns='http://www.w3.org/2000/svg'
                        enable-background='new 0 0 24 24'
                        height='24px'
                        viewBox='0 0 24 24'
                        width='24px'
                        fill='#000000'
                    >
                        <g>
                            <path d='M0,0h24v24H0V0z' fill='none'/>
                            <path d='M19.14,12.94c0.04-0.3,0.06-0.61,0.06-0.94c0-0.32-0.02-0.64-0.07-0.94l2.03-1.58c0.18-0.14,0.23-0.41,0.12-0.61 l-1.92-3.32c-0.12-0.22-0.37-0.29-0.59-0.22l-2.39,0.96c-0.5-0.38-1.03-0.7-1.62-0.94L14.4,2.81c-0.04-0.24-0.24-0.41-0.48-0.41 h-3.84c-0.24,0-0.43,0.17-0.47,0.41L9.25,5.35C8.66,5.59,8.12,5.92,7.63,6.29L5.24,5.33c-0.22-0.08-0.47,0-0.59,0.22L2.74,8.87 C2.62,9.08,2.66,9.34,2.86,9.48l2.03,1.58C4.84,11.36,4.8,11.69,4.8,12s0.02,0.64,0.07,0.94l-2.03,1.58 c-0.18,0.14-0.23,0.41-0.12,0.61l1.92,3.32c0.12,0.22,0.37,0.29,0.59,0.22l2.39-0.96c0.5,0.38,1.03,0.7,1.62,0.94l0.36,2.54 c0.05,0.24,0.24,0.41,0.48,0.41h3.84c0.24,0,0.44-0.17,0.47-0.41l0.36-2.54c0.59-0.24,1.13-0.56,1.62-0.94l2.39,0.96 c0.22,0.08,0.47,0,0.59-0.22l1.92-3.32c0.12-0.22,0.07-0.47-0.12-0.61L19.14,12.94z M12,15.6c-1.98,0-3.6-1.62-3.6-3.6 s1.62-3.6,3.6-3.6s3.6,1.62,3.6,3.6S13.98,15.6,12,15.6z'/>
                        </g>
                    </svg>
                </button>
                <button>
                    <svg
                        xmlns='http://www.w3.org/2000/svg'
                        height='24px'
                        viewBox='0 0 24 24'
                        width='24px'
                        fill='#000000'
                    >
                        <path d='M0 0h24v24H0V0z' fill='none'/>
                        <path d='M11 7h2v2h-2zm0 4h2v6h-2zm1-9C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'/>
                    </svg>
                </button>
                <button>
                    <svg
                        xmlns='http://www.w3.org/2000/svg'
                        height='24px'
                        viewBox='0 0 24 24'
                        width='24px'
                        fill='#000000'
                    >
                        <path d='M0 0h24v24H0V0z' fill='none' />
                        <path d='M22 9.24l-7.19-.62L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21 12 17.27 18.18 21l-1.63-7.03L22 9.24zM12 15.4l-3.76 2.27 1-4.28-3.32-2.88 4.38-.38L12 6.1l1.71 4.04 4.38.38-3.32 2.88 1 4.28L12 15.4z' />
                    </svg>
                </button>
            </div>
        </div>

        <nav class='mt-16'>
            <ul class='p-0 m-0'>
                <li>Store Page</li>
                <li>Community Hub</li>
                <li>Find Groups</li>
                <li>Discussions</li>
                <li>Guides</li>
                <li>Workshop</li>
                <li>Support</li>
            </ul>
        </nav>
    </section>
    <script src='./js/jquery-3.6.0.min.js'></script>
    <script src='./js/main.js'></script>
</body>

</html>

";
            return result;
        }
    }
}
